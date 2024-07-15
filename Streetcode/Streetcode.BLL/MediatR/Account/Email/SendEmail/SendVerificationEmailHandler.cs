using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Streetcode.BLL.Interfaces.Email;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Services.Email;
using Streetcode.DAL.Entities.Users;

namespace Streetcode.BLL.MediatR.Account.Email.SendEmail
{
    public class SendVerificationEmailHandler : IRequestHandler<SendVerificationEmailCommand, Result<string>>
    {
        private readonly ILoggerService _logger;
        private readonly ISendVerificationEmail _sender;
        public SendVerificationEmailHandler(
            ISendVerificationEmail sender,
            ILoggerService logger)
        {
            _sender = sender;
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
