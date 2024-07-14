using FluentResults;
using Streetcode.BLL.Behavior;
using Streetcode.BLL.DTO.Users;

namespace Streetcode.BLL.MediatR.Account.LoginWithGoogle;

public record LoginWithGoogleCommand(LoginWithGoogleDTO LoginWithGoogle) : IValidatableRequest<Result<UserDTO>>;