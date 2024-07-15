using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Interfaces.Email;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.URL;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Services.URL;
using Streetcode.DAL.Entities.AdditionalContent.Email;
using Streetcode.DAL.Entities.Users;

namespace Streetcode.BLL.MediatR.Account.RestorePassword
{
    public class RestorePasswordRequestHandler : IRequestHandler<RestorePasswordRequest, Result<string>>
    {        
        private readonly ILoggerService _loggerService;
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        private readonly EmailConfiguration _emailConfiguration;
        private readonly IURLGenerator _urlGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ResetPasswordConfiguration _resetPassConfigurtion;        

        public RestorePasswordRequestHandler(            
            ILoggerService logger,
            IEmailService emailService,
            IURLGenerator uRLGenerator,
            IHttpContextAccessor httpContextAccessor,
            UserManager<User> userManager,
            EmailConfiguration emailConfiguration,
            ResetPasswordConfiguration resetPasswordConfiguration)
        {           
            _loggerService = logger;
            _urlGenerator = uRLGenerator;
            _userManager = userManager;
            _emailService = emailService;
            _emailConfiguration = emailConfiguration;
            _httpContextAccessor = httpContextAccessor;
            _resetPassConfigurtion = resetPasswordConfiguration;
        }

        public async Task<Result<string>> Handle(RestorePasswordRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.restPassDto.Email);
            
            if (user == null)
            {
                var error = string.Format(ErrorMessages.UserWithEmailNotFound, request.restPassDto.Email);

                _loggerService.LogError(request, error);

                return Result.Fail(error);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            if (token == null) 
            {
                var error = ErrorMessages.FailToGenRestorePassToken;

                _loggerService.LogError(request, error);

                return Result.Fail(error);
            }

            Dictionary<string, object> key_value = new Dictionary<string, object>
            {
                { "userId", user.Id },
                { "token", token }
            };

            var url = _urlGenerator.Url(
                    _resetPassConfigurtion.RestorePassUrl!,
                    key_value,
                    _httpContextAccessor.HttpContext!);

            Message msg = new Message(
                new List<string>() { request.restPassDto.Email! },
                _emailConfiguration.From,
                "Streetcode Restore Password Request",
                "<div>Ви запросили відновлення паролю до вашого акаунту." +
                "Перейдіть за цим посиланням:</div>" +
                $"<div> <a href=\"{url}\">Відновити пароль!</a> </div>" +
                $"<div> <b>Якщо ви не відправляли запит на зміну паролю, то не переходьте за посиланням, перевірте свій АКАУНТ!</b></div>");

            bool success = await _emailService.SendEmailAsync(msg);

            if (!success)
            {
                var error = ErrorMessages.FailSendEmail;
                _loggerService.LogError(request, error);
                return Result.Fail(error);
            }

            return Result.Ok("Email for restoring password was sent successfuly.");
        }
    }
}
