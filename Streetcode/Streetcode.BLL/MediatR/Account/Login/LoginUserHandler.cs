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

namespace Streetcode.BLL.MediatR.Account.Login;

public class LoginUserHandler : IRequestHandler<LoginUserCommand, Result<UserDTO>>
{
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly ILoggerService _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICookieService _cookieService;
    private readonly TokensConfiguration _tokensConfiguration;

    public LoginUserHandler(
        UserManager<User> userManager, 
        SignInManager<User> signInManager, 
        ITokenService tokenService, 
        IMapper mapper, 
        ILoggerService logger, 
        IHttpContextAccessor httpContextAccessor, 
        ICookieService cookieService,
        TokensConfiguration tokensConfiguration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _mapper = mapper;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _cookieService = cookieService;
        _tokensConfiguration = tokensConfiguration;
    }

    public async Task<Result<UserDTO>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        
        var user = await _userManager.FindByNameAsync(request.LoginUser.Username);
        if (user == null)
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.InvalidUsernameOrPassword, request);
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.LoginUser.Password, false);
        if (!result.Succeeded)
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.InvalidUsernameOrPassword, request);
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var userDto = _mapper.Map<UserDTO>(user);
        if (userDto == null)
        {
            var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToMap, request);
            _logger.LogError(request, errorMsg);
            return Result.Fail(new Error(errorMsg));
        }

        var tokens = await _tokenService.GenerateTokens(user);

        await _cookieService.AppendCookiesToResponseAsync(httpContext.Response,
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
