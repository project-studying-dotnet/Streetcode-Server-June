using FluentValidation;

namespace Streetcode.BLL.MediatR.Account.RestorePassword
{
    public class RestorePasswordRequestDtoValidator : AbstractValidator<RestorePasswordRequest>
    {
        public RestorePasswordRequestDtoValidator()
        {
            RuleFor(x => x.restPassDto.Email).EmailAddress().NotEmpty();
        }
    }
}
