using FluentResults;
using Streetcode.BLL.Behavior;
using Streetcode.BLL.DTO.Users;

namespace Streetcode.BLL.MediatR.Account.RestorePassword;

public record RestorePasswordCommand(RestorePasswordDto ResetPasswordDto) : IValidatableRequest<Result<string>>;
