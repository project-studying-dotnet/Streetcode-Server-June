using FluentResults;
using Streetcode.BLL.Behavior;
using Streetcode.BLL.DTO.Comment;

namespace Streetcode.BLL.MediatR.Comments.Update;
public record UpdateCommentCommand(EditCommentDto EditedComment) : IValidatableRequest<Result<CommentDTO>>;
