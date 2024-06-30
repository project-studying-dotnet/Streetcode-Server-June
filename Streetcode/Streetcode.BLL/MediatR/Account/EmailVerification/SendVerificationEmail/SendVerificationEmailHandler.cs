using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Enums;
using Streetcode.BLL.Interfaces.Email;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.AdditionalContent.Email;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Account.EmailVerification.SendVerificationEmail
{
    public class SendVerificationEmailHandler : IRequestHandler<SendVerificationEmailQuery, Result<string>>
    {
        private readonly ILoggerService _logger;

        public SendVerificationEmailHandler(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILoggerService logger)
        {
            _logger = logger;
        }

        public async Task<Result<string>> Handle(SendVerificationEmailQuery request, CancellationToken cancellationToken)
        {
            UserRegisterDTO model = request.model;
            UserManager<IdentityUser> userManager = request.userManager;
            IEmailService emailSender = request.emailSender;
            ControllerBase controller = request.controller;

            if (!controller.ModelState.IsValid)
            {
                string errorMsg = "ModelState is Invalid";
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }

            var user = new IdentityUser { Email = model.Email, UserName = model.Email };
            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink =
                    controller.Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, controller.Request.Scheme);

                await emailSender
                    .SendEmailAsync(
                    new Message(new List<string> { model.Email }, "Streetcode", "Confirm your email", confirmationLink!));

                return Result.Ok("The message was sent to the email");
            }
            else
            {
                string errorMsg = MessageResourceContext.GetMessage(ErrorMessages.FailSendEmail, request);
                _logger.LogError(request, errorMsg);
                return Result.Fail(new Error(errorMsg));
            }
        }
    }
}
