using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Interfaces.Email;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Account.Register;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Users;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Account.EmailVerification.ConfirmEmail
{
    public class ConfirmUserEmailHandler : IRequestHandler<ConfirmUserEmailCommand, Result<string>>
    {
        private readonly ILoggerService _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public ConfirmUserEmailHandler(UserManager<IdentityUser> userManager, ILoggerService logger)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<Result<string>> Handle(ConfirmUserEmailCommand request, CancellationToken cancellationToken)
        {
            string userId = request.userId;
            string token = request.token;

            if (userId == null || token == null)
            {
                string errorMsg = "userId or token is empty";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                string errorMsg = MessageResourceContext.GetMessage(ErrorMessages.EntityNotFound, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return Result.Ok("Email is confirmed");
            }
            else
            {
                string errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailToCreateA, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }
        }
    }
}
