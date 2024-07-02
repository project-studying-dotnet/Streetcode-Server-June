using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Users;

namespace Streetcode.BLL.MediatR.Account.Logout
{
    public record LogoutUserCommand : IRequest<Result<string>>;
}
