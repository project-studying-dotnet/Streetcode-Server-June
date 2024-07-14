using FluentValidation;

namespace Streetcode.BLL.MediatR.Account.LoginWithGoogle;

public class LoginWithGoogleRequestDTOValidator : AbstractValidator<LoginWithGoogleCommand>
{
    public LoginWithGoogleRequestDTOValidator()
    {
        RuleFor(u => u.LoginWithGoogle.IdToken).NotNull().NotEmpty();
    }
}