using FluentResults;
using Streetcode.BLL.Behavior;
using Streetcode.BLL.DTO.Comment;

namespace Streetcode.BLL.MediatR.Replies;

public record CreateReplyCommand(ReplyCreateDTO reply) : IValidatableRequest<Result<ReplyDTO>>;
