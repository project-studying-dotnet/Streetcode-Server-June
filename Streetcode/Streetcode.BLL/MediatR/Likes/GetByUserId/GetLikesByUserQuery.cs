using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Likes;

namespace Streetcode.BLL.MediatR.Likes.PushLike
{
    public record GetLikesByUserQuery : IRequest<Result<IEnumerable<LikeDTO>>>;
}
