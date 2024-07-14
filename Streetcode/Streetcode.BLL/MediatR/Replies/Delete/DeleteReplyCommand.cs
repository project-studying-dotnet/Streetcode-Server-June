using FluentResults;
using Streetcode.BLL.Behavior;
using Streetcode.BLL.DTO.Comment;

namespace Streetcode.BLL.MediatR.Replies.Delete
{
    public record DeleteReplyCommand(int Id) : IValidatableRequest<Result<CommentDTO>>;
}
