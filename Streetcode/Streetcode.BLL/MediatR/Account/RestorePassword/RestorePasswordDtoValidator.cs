using FluentValidation;
using Streetcode.BLL.DTO.Users;

namespace Streetcode.BLL.MediatR.Account.RestorePassword;

public class RestorePasswordDtoValidator : AbstractValidator<RestorePasswordCommand>
{
    public RestorePasswordDtoValidator()
    {
        RuleFor(x => x.ResetPasswordDto.UserId).NotNull().NotEmpty().Must(BeAValidGuid);
        RuleFor(x => x.ResetPasswordDto.Token).NotNull().NotEmpty();
        RuleFor(x => x.ResetPasswordDto.NewPassword).MinimumLength(7);
    }

    private bool BeAValidGuid(string userId)
    {
        return Guid.TryParse(userId, out _);
    }
}