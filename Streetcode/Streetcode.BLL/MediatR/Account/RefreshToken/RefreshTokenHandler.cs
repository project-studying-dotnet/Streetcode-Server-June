using System.Security.Claims;
using Castle.Core.Internal;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Services.Tokens;
using Streetcode.DAL.Entities.Users;

namespace Streetcode.BLL.MediatR.Account.RefreshTokens
{
    public class RefreshTokensHandler : IRequestHandler<RefreshTokensCommand, Result<User>>
    {
        private readonly ILoggerService _logger;
        private TokenService _tokenService;
        private UserManager<User> _userManager;
        private TokensConfiguration _tokensConfiguration;

        public RefreshTokensHandler(UserManager<User> userManager, ILoggerService logger, TokensConfiguration accessTokenConfiguration)
        {
            _logger = logger;
            _userManager = userManager;
            _tokensConfiguration = accessTokenConfiguration;
            _tokenService = new(_userManager, _tokensConfiguration, _logger);
        }

        public async Task<Result<User>> Handle(RefreshTokensCommand request, CancellationToken cancellationToken)
        {
            // Get token response
            if(request.tokenResponse == null)
            {
                var errorMsgNull = MessageResourceContext.GetMessage(ErrorMessages.EntityNotFound, request);
                _logger.LogError(request, errorMsgNull);
                return Result.Fail(new Error(errorMsgNull));
            }

            var tokens = request.tokenResponse;

            string accessToken = tokens.AccessToken;

            // Find user
            var claims = _tokenService.GetPrincipalFromAccessToken(accessToken);

            string email = claims.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                var errorMsgNull = MessageResourceContext.GetMessage(ErrorMessages.EntityNotFound, request);
                _logger.LogError(request, errorMsgNull);
                return Result.Fail(new Error(errorMsgNull));
            }

            var user = await _userManager.FindByEmailAsync(email);
            if(user == null)
            {
                var errorMsgNull = MessageResourceContext.GetMessage(ErrorMessages.UserNotFound, request);
                _logger.LogError(request, errorMsgNull);
                return Result.Fail(new Error(errorMsgNull));
            }

            // Generate and implement new tokens
            tokens = await _tokenService.GenerateTokens(user);
            if(tokens.AccessToken == null || tokens.RefreshToken.Token == null)
            {
                var errorMsgNull = MessageResourceContext.GetMessage(ErrorMessages.InvalidToken, request);
                _logger.LogError(request, errorMsgNull);
                return Result.Fail(new Error(errorMsgNull));
            }

            return Result.Ok(user);
        }
    }
}
