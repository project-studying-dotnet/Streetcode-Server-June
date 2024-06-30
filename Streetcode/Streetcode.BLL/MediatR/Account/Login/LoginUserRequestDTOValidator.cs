using FluentValidation;

namespace Streetcode.BLL.MediatR.Account.Login;

public class LoginUserRequestDTOValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserRequestDTOValidator()
    {
        RuleFor(u => u.LoginUser.Login).EmailAddress();
        RuleFor(u => u.LoginUser.Password).MinimumLength(7);
    }
}