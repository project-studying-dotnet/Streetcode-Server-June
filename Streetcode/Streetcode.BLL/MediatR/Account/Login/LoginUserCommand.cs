using FluentResults;
using Streetcode.BLL.Behavior;
using Streetcode.BLL.DTO.Users;

namespace Streetcode.BLL.MediatR.Account.Login;

public record LoginUserCommand(UserLoginDTO LoginUser) : IValidatableRequest<Result<UserDTO>>;