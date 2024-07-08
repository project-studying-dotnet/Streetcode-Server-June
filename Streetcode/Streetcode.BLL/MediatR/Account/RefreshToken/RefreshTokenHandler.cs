using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Services.CookieService.Interfaces;
using Streetcode.BLL.Services.Tokens;
using Streetcode.DAL.Entities.Users;

namespace Streetcode.BLL.MediatR.Account.RefreshToken
{
    public class RefreshTokensHandler : IRequestHandler<RefreshTokensCommand, Result<string>>
    {
        private readonly ILoggerService _logger;
        private readonly ITokenService _tokenService;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICookieService _cookieService;
        private readonly TokensConfiguration _tokensConfiguration;

        public RefreshTokensHandler(
            UserManager<User> userManager, 
            ILoggerService logger, 
            ITokenService tokenService, 
            IHttpContextAccessor httpContextAccessor,
            ICookieService cookieService,
            TokensConfiguration tokensConfiguration)
        {
            _logger = logger;
            _userManager = userManager;
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
            _cookieService = cookieService;
            _tokensConfiguration = tokensConfiguration;
        }

        public async Task<Result<string>> Handle(RefreshTokensCommand request, CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (!httpContext!.Request.Cookies.TryGetValue("refreshToken", out var refreshToken) && string.IsNullOrEmpty(refreshToken))
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.RefreshTokenNotFound, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var user = _userManager.Users
                .Where(u => u.RefreshTokens.Any(t => t.Token == refreshToken && t.Expires > DateTime.UtcNow))
                .Include(u => u.RefreshTokens)
                .FirstOrDefault();

            if (user is null)
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.RefreshTokenNotFound, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            user.RefreshTokens.RemoveAll(t => t.Token == refreshToken);

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

            return Result.Ok("Tokens refreshed successfully!");
        }
    }
}
