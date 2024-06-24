using FluentValidation;
using Streetcode.BLL.DTO.Email;
namespace Streetcode.BLL.MediatR.Email;

public class SendEmailRequestDTOValidator : AbstractValidator<SendEmailCommand>
{
    public SendEmailRequestDTOValidator()
    {
        RuleFor(x => x.Email.From)
            .MaximumLength(80).EmailAddress();

        RuleFor(x => x.Email.Content)
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(500);
    }
}