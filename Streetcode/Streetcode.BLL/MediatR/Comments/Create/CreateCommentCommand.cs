using FluentResults;
using Streetcode.BLL.Behavior;
using Streetcode.BLL.DTO.Comment;

namespace Streetcode.BLL.MediatR.Comments.Create
{
    public record CreateCommentCommand(CommentCreateDTO comment) : IValidatableRequest<Result<CommentDTO>>;
}
