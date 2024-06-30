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
    public class ConfirmUserEmailHandler : IRequestHandler<ConfirmUserEmailQuery, Result<string>>
    {
        private readonly ILoggerService _logger;

        public ConfirmUserEmailHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _logger = logger;
        }

        public async Task<Result<string>> Handle(ConfirmUserEmailQuery request, CancellationToken cancellationToken)
        {
            string userId = request.userId;
            string token = request.token;
            UserManager<IdentityUser> userManager = request.userManager; // it may be better to call it through the constructor

            if (userId == null || token == null)
            {
                string errorMsg = "userId or token is empty";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                string errorMsg = MessageResourceContext.GetMessage(ErrorMessages.EntityNotFound, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var result = await userManager.ConfirmEmailAsync(user, token);
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
