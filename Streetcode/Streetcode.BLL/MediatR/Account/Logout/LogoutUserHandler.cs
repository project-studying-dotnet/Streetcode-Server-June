using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Services.CacheService;
using Streetcode.DAL.Entities.Users;

namespace Streetcode.BLL.MediatR.Account.Logout
{
    public class LogoutUserHandler : IRequestHandler<LogoutUserCommand, Result<string>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ICacheService _cacheService;
        private readonly ILoggerService _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenService _tokenService;

        public LogoutUserHandler(UserManager<User> userManager, ICacheService cacheService, ILoggerService logger, IHttpContextAccessor httpContextAccessor, ITokenService tokenService)
        {
            _userManager = userManager;
            _cacheService = cacheService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _tokenService = tokenService;
        }

        public async Task<Result<string>> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (!httpContext!.Request.Cookies.TryGetValue("accessToken", out var accessToken) || string.IsNullOrEmpty(accessToken))
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.AccessTokenNotFound, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }
            
            if (string.IsNullOrEmpty(accessToken))
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.AccessTokenNotFound, request);
                _logger.LogError(accessToken!, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var userId = _tokenService.GetUserIdFromAccessToken(accessToken);
            var user = await _userManager.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.Id == new Guid(userId));
            if (user == null)
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.UserNotFound, request);
                _logger.LogError(user!, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            if (httpContext!.Request.Cookies.TryGetValue("refreshToken", out var refreshToken) || !string.IsNullOrEmpty(refreshToken))
            {
                var refreshTokenEntity = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);
                if (refreshTokenEntity != null)
                {
                    user.RefreshTokens.Remove(refreshTokenEntity);
                }
            }

            ClearCookies();
       
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.UserUpdateFailed, request);
                _logger.LogError(result!, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var cacheResult = await _cacheService.SetBlacklistedTokenAsync(accessToken, user.Id.ToString());
            if (!cacheResult)
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailedToSetTokenInBlackList, request);
                _logger.LogError(cacheResult, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }
            
            return Result.Ok("User logged out successfully");
        }
        
        private void ClearCookies()
        {
            foreach (var cookie in _httpContextAccessor.HttpContext!.Request.Cookies.Keys)
            {
                _httpContextAccessor.HttpContext!.Response.Cookies.Delete(cookie, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(-1),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None
                });
            }
        }
    }
}
