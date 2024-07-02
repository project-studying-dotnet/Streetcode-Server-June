using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Streetcode.BLL.DTO.Users;
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
        private readonly IMapper _mapper;
        public LogoutUserHandler(UserManager<User> userManager, ICacheService cacheService, ILoggerService logger, IHttpContextAccessor httpContextAccessor, IMapper mapper, ITokenService tokenService)
        {
            _userManager = userManager;
            _cacheService = cacheService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _tokenService = tokenService;
            _mapper = mapper;
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

            var userId = await _tokenService.GetUserIdFromAccessToken(accessToken);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.UserNotFound, request);
                _logger.LogError(user!, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            user.RefreshToken = null!;

            await _userManager.UpdateAsync(user);

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
