using FluentResults;
using Streetcode.BLL.Behavior;
using Streetcode.BLL.DTO.Streetcode.TextContent.RelatedTerm;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedTerm.DeleteById;

public record DeleteRelatedTermByIdCommand(int Id) : IValidatableRequest<Result<RelatedTermDTO>>;