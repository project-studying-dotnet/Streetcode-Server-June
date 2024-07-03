using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Streetcode.BLL.Interfaces.Email;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Util.Account;
using Streetcode.DAL.Entities.Users;

namespace Streetcode.BLL.MediatR.Account.EmailVerification.SendEmail
{
    public class SendVerificationEmailHandler : IRequestHandler<SendVerificationEmailCommand, Result<string>>
    {
        private readonly ILoggerService _logger;
        private readonly SendVerificationEmail _sender;
        public SendVerificationEmailHandler(
            UserManager<User> userManager,
            IEmailService emailSender,
            ILoggerService logger,
            IUrlHelper urlHelper)
        {
            _sender = new SendVerificationEmail(userManager, emailSender, urlHelper);
            _logger = logger;
        }

        public async Task<Result<string>> Handle(SendVerificationEmailCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _sender.SendVerification(request.email);
                return Result.Ok(request.email);
            }
            catch (Exception ex)
            {
                string errorMsg = ex.Message;
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }
        }
    }
}
