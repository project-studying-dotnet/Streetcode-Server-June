using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Comment;

namespace Streetcode.WebApi.Controllers.Comment
{
    public record GetAllCommentsByStreetcodeIdQuery(int streetcodeId) : IRequest<Result<IEnumerable<CommentDTO>>>;
}