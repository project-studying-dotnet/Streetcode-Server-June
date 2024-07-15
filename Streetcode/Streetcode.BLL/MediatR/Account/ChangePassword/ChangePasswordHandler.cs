using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Services.CacheService;
using Streetcode.BLL.Services.CookieService.Interfaces;
using Streetcode.DAL.Entities.Users;

namespace Streetcode.BLL.MediatR.Account.ChangePassword
{
    public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, Result<string>>
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ICacheService _cacheService;
        private readonly ITokenService _tokenService;
        private readonly ILoggerService _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICookieService _cookieService;

        public ChangePasswordHandler(
            UserManager<User> userManager,
            ICacheService cacheService,
            ITokenService tokenService,
            ILoggerService logger,
            IHttpContextAccessor httpContextAccessor,
            ICookieService cookieService,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _cacheService = cacheService;
            _tokenService = tokenService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _cookieService = cookieService;
            _signInManager = signInManager;
        }

        public async Task<Result<string>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (!httpContext!.Request.Cookies.TryGetValue("accessToken", out var accessToken))
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.AccessTokenNotFound, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var userId = _tokenService.GetUserIdFromAccessToken(accessToken);
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.UserNotFound, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var result = await _userManager.ChangePasswordAsync(user, request.PasswordChange.CurrentPassword, request.PasswordChange.NewPassword);

            if (!result.Succeeded)
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToChangePassword, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var cacheResult = await _cacheService.SetBlacklistedTokenAsync(accessToken, user.Id.ToString());
            if (!cacheResult)
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailedToSetTokenInBlackList, request);
                _logger.LogError(cacheResult, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            user.RefreshTokens.Clear();

            await _cookieService.ClearRequestCookiesAsync(_httpContextAccessor.HttpContext);

            await _signInManager.SignOutAsync();
            return Result.Ok("Password changed successfully");
        }
    }
}
