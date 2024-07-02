using FluentResults;
using Streetcode.BLL.Behavior;
using Streetcode.BLL.DTO.Users;

namespace Streetcode.BLL.MediatR.Account.Logout
{
    public record LogoutUserCommand : IValidatableRequest<Result<UserDTO>>;
}
