using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Services.CacheService;
using Streetcode.BLL.Services.CookieService.Interfaces;
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
        private readonly ICookieService _cookieService;

        public LogoutUserHandler(UserManager<User> userManager, ICacheService cacheService, ILoggerService logger, IHttpContextAccessor httpContextAccessor, ITokenService tokenService, ICookieService Cookieservice)
        {
            _userManager = userManager;
            _cacheService = cacheService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _tokenService = tokenService;
            _cookieService = Cookieservice;
        }

        public async Task<Result<string>> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            
            if (!httpContext!.Request.Cookies.TryGetValue("accessToken", out var accessToken))
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.AccessTokenNotFound, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            if (string.IsNullOrEmpty(accessToken))
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.AccessTokenNotFound, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var userId = _tokenService.GetUserIdFromAccessToken(accessToken);
            var user = await _userManager.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.Id == new Guid(userId));

            if (httpContext!.Request.Cookies.TryGetValue("refreshToken", out var refreshToken) && !string.IsNullOrEmpty(refreshToken))
            {
                var refreshTokenEntity = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);
                if (refreshTokenEntity != null)
                {
                    user.RefreshTokens.Remove(refreshTokenEntity);
                }
            }

            await _cookieService.ClearRequestCookiesAsync(_httpContextAccessor.HttpContext);
       
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
    }
}
