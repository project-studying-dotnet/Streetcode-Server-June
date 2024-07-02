using FluentValidation;

namespace Streetcode.BLL.MediatR.Account.EmailVerification.SendEmail
{
    public class SendVerificationEmailDTOValidator : AbstractValidator<SendVerificationEmailCommand>
    {
        public SendVerificationEmailDTOValidator()
        {
            RuleFor(u => u.email).EmailAddress();
        }
    }
}
