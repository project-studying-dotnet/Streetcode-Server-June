using FluentResults;
using MediatR;

namespace Streetcode.BLL.MediatR.Account.RefreshToken
{
    public record RefreshTokensCommand : IRequest<Result<string>>;
}
