using FluentResults;
using Streetcode.BLL.Behavior;
using Streetcode.BLL.DTO.Users;
using Streetcode.DAL.Entities.Users;

namespace Streetcode.BLL.MediatR.Account.RefreshTokens
{
    public record RefreshTokensCommand(TokenResponseDTO tokenResponse) : IValidatableRequest<Result<User>>;
}
