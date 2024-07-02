using FluentResults;
using MediatR;
using Streetcode.DAL.Entities.Users;

namespace Streetcode.BLL.MediatR.Account.RefreshTokens
{
    public record RefreshTokensCommand : IRequest<Result<string>>;
}
