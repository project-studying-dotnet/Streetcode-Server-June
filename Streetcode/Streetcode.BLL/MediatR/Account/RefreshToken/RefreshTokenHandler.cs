using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Users;

namespace Streetcode.BLL.MediatR.Account.RefreshToken
{
    public class RefreshTokensHandler : IRequestHandler<RefreshTokensCommand, Result<string>>
    {
        private readonly ILoggerService _logger;
        private readonly ITokenService _tokenService;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RefreshTokensHandler(UserManager<User> userManager, ILoggerService logger, ITokenService tokenService, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _userManager = userManager;
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<string>> Handle(RefreshTokensCommand request, CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (!httpContext!.Request.Cookies.TryGetValue("refreshToken", out var refreshToken) || string.IsNullOrEmpty(refreshToken))
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.RefreshTokenNotFound, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var user = _userManager.Users.FirstOrDefault(c => c.RefreshToken == refreshToken);
            if (user is null)
            {
                var errorMsg = MessageResourceContext.GetMessage(ErrorMessages.RefreshTokenNotFound, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }
            
            await _tokenService.GenerateAndSetTokensAsync(user, httpContext.Response);
            
            return Result.Ok("Tokens refreshed successfully!");
        }
    }
}
