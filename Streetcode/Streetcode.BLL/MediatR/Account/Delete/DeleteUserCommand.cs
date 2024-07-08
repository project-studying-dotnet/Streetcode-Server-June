using FluentResults;
using MediatR;
using Streetcode.BLL.Behavior;
using Streetcode.BLL.DTO.Users;

namespace Streetcode.BLL.MediatR.Account.Delete
{
    public record class DeleteUserCommand() : IValidatableRequest<Result<DeleteUserResponseDto>>;    
}
