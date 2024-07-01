using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Services.Tokens;
using Streetcode.DAL.Entities.Users;
using Streetcode.DAL.Enums;

namespace Streetcode.BLL.MediatR.Account.Register
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Result<UserDTO>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;
        private readonly ITokenService _tokenService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly TokensConfiguration _tokensConfiguration;


        public RegisterUserHandler(IMapper mapper, ILoggerService logger, UserManager<User> userManager, TokensConfiguration tokensConfiguration, ITokenService tokenService, IHttpContextAccessor context)
        {
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
            _tokensConfiguration = tokensConfiguration;
            _tokenService = tokenService;
            _contextAccessor = context;
        }

        public async Task<Result<UserDTO>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var req = request.newUser;
            if (await IsEmailUse(req.Email))
            {
                var errorMessage = MessageResourceContext.GetMessage(ErrorMessages.EmailIsUse, request);
                _logger.LogError(request, errorMessage);

                return Result.Fail(new Error(errorMessage));
            }

            if (await IsLoginUse(req.Login))
            {
                var errorMessage = MessageResourceContext.GetMessage(ErrorMessages.LoginIsUse, request);
                _logger.LogError(request, errorMessage);

                return Result.Fail(errorMessage);
            }

            var user = _mapper.Map<User>(req);

            IdentityResult newUser = await _userManager.CreateAsync(user, req.Password);

            if (!newUser.Succeeded)
            {
                var errorMessage = MessageResourceContext.GetMessage(ErrorMessages.FailCreateUser, request);

                _logger.LogError(request, errorMessage);

                return Result.Fail(errorMessage);
            }

            IdentityResult addingRoleResult = await _userManager.AddToRoleAsync(user, UserRole.User.ToString());

            if (!addingRoleResult.Succeeded)
            {
                var errorMessage = MessageResourceContext.GetMessage(ErrorMessages.FailAddRole, request);

                _logger.LogError(request, errorMessage);

                return Result.Fail(errorMessage);
            }

            var tokens = await _tokenService.GenerateTokens(user);
            _contextAccessor.HttpContext!.Response.Cookies.Append("accessToken", tokens.AccessToken, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddMinutes(_tokensConfiguration.AccessTokenExpirationMinutes),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });
            _contextAccessor.HttpContext!.Response.Cookies.Append("refreshToken", tokens.RefreshToken.Token, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(_tokensConfiguration.RefreshTokenExpirationDays),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });

            return Result.Ok(_mapper.Map<UserDTO>(user));
        }

        private async Task<bool> IsEmailUse(string email)
        {
            User? user = await _userManager.FindByEmailAsync(email);

            return user is not null;
        }

        private async Task<bool> IsLoginUse(string login)
        {
            User? user = await _userManager.FindByNameAsync(login);

            return user is not null;
        }
    }
}