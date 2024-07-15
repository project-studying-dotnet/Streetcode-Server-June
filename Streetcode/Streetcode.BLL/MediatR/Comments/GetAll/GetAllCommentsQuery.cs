using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Comment;

namespace Streetcode.BLL.MediatR.Comments.GetAll
{
    public record GetAllCommentsQuery : IRequest<Result<IEnumerable<CommentDTO>>>;
}
