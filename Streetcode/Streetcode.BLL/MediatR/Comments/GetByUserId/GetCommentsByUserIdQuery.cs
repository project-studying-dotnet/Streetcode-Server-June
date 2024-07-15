using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Comment;

namespace Streetcode.BLL.MediatR.Comments.GetByUserId
{
    public record GetCommentsByUserIdQuery(Guid UserId)
        : IRequest<Result<IEnumerable<CommentDTO>>>;
}
