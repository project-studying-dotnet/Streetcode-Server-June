using FluentValidation;

namespace Streetcode.BLL.MediatR.Account.Login;

public class LoginUserRequestDTOValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserRequestDTOValidator()
    {
        RuleFor(u => u.LoginUser.Username).NotEmpty().Length(4, 256);
        RuleFor(u => u.LoginUser.Password).MinimumLength(7);
    }
}