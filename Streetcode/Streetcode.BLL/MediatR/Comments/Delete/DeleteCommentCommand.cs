using FluentResults;
using Streetcode.BLL.Behavior;
using Streetcode.BLL.DTO.Comment;

namespace Streetcode.BLL.MediatR.Comments.Delete
{
    public record DeleteCommentCommand(int commentId) : IValidatableRequest<Result<CommentDTO>>;
}
