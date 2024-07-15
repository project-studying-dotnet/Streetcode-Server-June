using FluentValidation;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.MediatR.Account.Register
{
    public class RegisterUserRequestDTOValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserRequestDTOValidator()
        {
            RuleFor(u => u.newUser.Email).EmailAddress();
            RuleFor(u => u.newUser.Password).MinimumLength(9);
            RuleFor(u => u.newUser.Password).Matches("[A-Z]").WithMessage(ErrorMessages.MustContainUpperCase);
            RuleFor(u => u.newUser.Password).Matches("[a-z]").WithMessage(ErrorMessages.MustContainLowerCase);
            RuleFor(u => u.newUser.Password).Matches("[0-9]").WithMessage(ErrorMessages.MustContainNumber);
            RuleFor(u => u.newUser.Password).Matches("[^a-zA-Z0-9]").WithMessage(ErrorMessages.MustContainSpecialSymbol);
            RuleFor(u => u.newUser.Username).NotEmpty().Length(4, 256);

            // Add more if needed
        }
    }
}
