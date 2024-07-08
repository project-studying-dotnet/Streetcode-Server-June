using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using StackExchange.Redis;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Services.CacheService;
using Streetcode.BLL.Services.CookieService.Interfaces;
using Streetcode.DAL.Entities.Users;

namespace Streetcode.BLL.MediatR.Account.Delete
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result<DeleteUserResponseDto>>
    {
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICacheService _cacheService;
        private readonly ICookieService _cookieService;
        private readonly ITokenService _tokenService;

        public DeleteUserCommandHandler(
            IMapper mapper, 
            ILoggerService logger, 
            UserManager<User> userManager, 
            IHttpContextAccessor accesor, 
            ICacheService cache, 
            ICookieService cookieService,
            ITokenService tokenService)
        {
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
            _contextAccessor = accesor;
            _cacheService = cache;
            _cookieService = cookieService;
            _tokenService = tokenService;
        }

        public async Task<Result<DeleteUserResponseDto>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var httpContext = _contextAccessor.HttpContext;

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

            await _cookieService.ClearRequestCookiesAsync(_contextAccessor.HttpContext);
            
            var userId = _tokenService.GetUserIdFromAccessToken(accessToken);

            User? user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                var error = string.Format(ErrorMessages.UserWithIdNotFound, userId);

                _logger.LogError(request, error);

                return Result.Fail(error);
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                var error = result.Errors.Select(e => e.Description).Aggregate((e1, e2) => "- " + e1 + "\n" + "- " + e2 + "\n");

                _logger.LogError(request, error);

                return Result.Fail(error);
            }

            var cacheResult = await _cacheService.SetBlacklistedTokenAsync(accessToken, user.Id.ToString());
            if (!cacheResult)
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailedToSetTokenInBlackList, request);
                _logger.LogError(cacheResult, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            return Result.Ok(_mapper.Map<DeleteUserResponseDto>(user));
        }
    }
}
