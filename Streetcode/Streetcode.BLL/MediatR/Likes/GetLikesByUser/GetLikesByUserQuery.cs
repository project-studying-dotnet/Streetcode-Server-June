using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Likes;
using Streetcode.BLL.DTO.Streetcode;

namespace Streetcode.BLL.MediatR.Likes.PushLike
{
    public record GetLikesByUserQuery : IRequest<Result<IEnumerable<StreetcodeDTO>>>;
}
