﻿using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.BLL.MediatR.Account.Login;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Services.CacheService;
using Streetcode.BLL.Services.CookieService.Interfaces;
using Streetcode.BLL.Services.Tokens;
using Streetcode.DAL.Entities.Users;


namespace Streetcode.BLL.MediatR.Account.ChangePassword
{
    public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, Result<string>>
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ICacheService _cacheService;
        private readonly ITokenService _tokenService;
        private readonly ILoggerService _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICookieService _cookieService;
        private readonly TokensConfiguration _tokensConfiguration;

        public ChangePasswordHandler(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ICacheService cacheService,
            ITokenService tokenService,
            IMapper mapper,
            ILoggerService logger,
            IHttpContextAccessor httpContextAccessor,
            ICookieService cookieService,
            TokensConfiguration tokensConfiguration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _cacheService = cacheService;
            _tokenService = tokenService;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _cookieService = cookieService;
            _tokensConfiguration = tokensConfiguration;
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
            var user = await _userManager.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.Id == new Guid(userId));

            if (user is null)
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.AccessTokenNotFound, request);
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

            // remove tokens cookies and logout after successfully changing password

            var cacheResult = await _cacheService.SetBlacklistedTokenAsync(accessToken, user.Id.ToString());
            if (!cacheResult)
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailedToSetTokenInBlackList, request);
                _logger.LogError(cacheResult, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            await _cookieService.ClearRequestCookiesAsync(_httpContextAccessor.HttpContext);

            var logoutResult = await _userManager.UpdateAsync(user);
            if (!logoutResult.Succeeded)
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.UserUpdateFailed, request);
                _logger.LogError(logoutResult, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            return Result.Ok("Password changed successfully");
        }
    }
}
