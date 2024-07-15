using System.Text.RegularExpressions;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Services.CookieService.Interfaces;
using Streetcode.BLL.Services.Tokens;
using Streetcode.DAL.Entities.Users;
using Streetcode.DAL.Enums;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace Streetcode.BLL.MediatR.Account.LoginWithGoogle;

public class LoginWithGoogleHandler : IRequestHandler<LoginWithGoogleCommand, Result<UserDTO>>
{
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;
    private readonly ILoggerService _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICookieService _cookieService;
    private readonly TokensConfiguration _tokensConfiguration;

    public LoginWithGoogleHandler(
        UserManager<User> userManager,
        ITokenService tokenService,
        IMapper mapper,
        ILoggerService logger,
        IHttpContextAccessor httpContextAccessor,
        ICookieService cookieService,
        TokensConfiguration tokensConfiguration)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _mapper = mapper;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _cookieService = cookieService;
        _tokensConfiguration = tokensConfiguration;
    }

    public async Task<Result<UserDTO>> Handle(LoginWithGoogleCommand request, CancellationToken cancellationToken)
    {
        Payload googleCredentials;
        try
        {
            googleCredentials = await ValidateAsync(request.LoginWithGoogle.IdToken, new ValidationSettings { Audience = new List<string> { _tokensConfiguration.GoogleClientId } });
        }
        catch (Exception)
        {

            var errorMessage = MessageResourceContext.GetMessage(ErrorMessages.InvalidToken, request);
            _logger.LogError(request, errorMessage);
            return Result.Fail(errorMessage);
        }

        var user = await _userManager.FindByEmailAsync(googleCredentials.Email);
        if (user == null)
        {
            var createUser = new User
            {
                UserName = Regex.Replace(googleCredentials.Name, "[^a-zA-Z0-9]", string.Empty),
                FirstName = googleCredentials.GivenName,
                LastName = googleCredentials.FamilyName,
                Email = googleCredentials.Email,
                BirthDate = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            var isUserNameInUse = await _userManager.FindByNameAsync(createUser.UserName) is not null;

            if (isUserNameInUse)
            {
                createUser.UserName = $"{createUser.UserName}_{DateTime.UtcNow.Ticks.ToString()[..6]}";
            }

            var newUserResult = await _userManager.CreateAsync(createUser);
            if (!newUserResult.Succeeded)
            {
                var errorMessage = MessageResourceContext.GetMessage(ErrorMessages.FailCreateUser, request);
                _logger.LogError(request, errorMessage);
                return Result.Fail(errorMessage);
            }
            else
            {
                user = createUser;
            }

            IdentityResult addingRoleResult = await _userManager.AddToRoleAsync(user, UserRole.User.ToString());
            if (!addingRoleResult.Succeeded)
            {
                var errorMessage = MessageResourceContext.GetMessage(ErrorMessages.FailAddRole, request);
                _logger.LogError(request, errorMessage);
                return Result.Fail(errorMessage);
            }
        }

        var userDto = _mapper.Map<UserDTO>(user);
        if (userDto == null)
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToMap, request);
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var tokens = await _tokenService.GenerateTokens(user);

        await _cookieService.AppendCookiesToResponseAsync(_httpContextAccessor.HttpContext.Response,
            ("accessToken", tokens.AccessToken, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddMinutes(_tokensConfiguration.AccessTokenExpirationMinutes),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            }),
            ("refreshToken", tokens.AccessToken, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(_tokensConfiguration.RefreshTokenExpirationDays),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            }));

        await _tokenService.SetRefreshToken(tokens.RefreshToken, user);

        return Result.Ok(userDto);
    }
}
