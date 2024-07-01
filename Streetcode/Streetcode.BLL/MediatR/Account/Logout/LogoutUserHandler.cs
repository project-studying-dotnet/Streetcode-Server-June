using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Services.CacheService;
using Streetcode.DAL.Entities.Users;

namespace Streetcode.BLL.MediatR.Account.Logout
{
    public class LogoutUserHandler : IRequestHandler<LogoutUserCommand, Result<UserDTO>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ICacheService _cacheService;
        private readonly ILoggerService _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        public LogoutUserHandler(UserManager<User> userManager,
            ICacheService cacheService, ILoggerService logger,
            IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _mapper = mapper;
            _userManager = userManager;
            _cacheService = cacheService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<UserDTO>> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
        {
            var accessToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            
            if (string.IsNullOrEmpty(accessToken))
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.AccessTokenNotFound, request);
                _logger.LogError(accessToken!, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }
            
            var claims = _httpContextAccessor.HttpContext?.User.Claims.ToList();
            if(!claims!.Any())
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.ClaimsNotExist, request);
                _logger.LogError(claims!, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }
            
            var userIdClaim = claims!.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            var user = await _userManager.FindByIdAsync(userIdClaim);
            if (user == null)
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.UserNotFound, request);
                _logger.LogError(user!, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            user.RefreshToken = null!;

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
            
            return Result.Ok(_mapper.Map<UserDTO>(user));
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
