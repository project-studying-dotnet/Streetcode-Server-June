using FluentResults;
using Streetcode.BLL.Behavior;
using Streetcode.BLL.DTO.Users;

namespace Streetcode.BLL.MediatR.Account.ChangePassword
{
    public record ChangePasswordCommand(ChangePasswordDTO PasswordChange)
        : IValidatableRequest<Result<string>>;
}
