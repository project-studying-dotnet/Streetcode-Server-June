using FluentResults;
using Streetcode.BLL.Behavior;
using Streetcode.BLL.DTO.Likes;

namespace Streetcode.BLL.MediatR.Likes.PushLike
{
    public record PushLikeCommand(PushLikeDTO pushLike) : IValidatableRequest<Result<LikeDTO>>;
}
