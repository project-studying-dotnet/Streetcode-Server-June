using FluentResults;
using Streetcode.BLL.Behavior;
using Streetcode.BLL.DTO.Likes;

namespace Streetcode.BLL.MediatR.Likes.PushLike
{
    public record GetLikesByUserIdQuery(Guid userId) : IValidatableRequest<Result<IEnumerable<LikeDTO>>>;
}
