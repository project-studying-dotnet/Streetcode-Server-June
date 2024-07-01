using FluentResults;
using Streetcode.BLL.Behavior;
using Streetcode.BLL.DTO.Streetcode.TextContent.RelatedTerm;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Create;

public record CreateRelatedTermCommand(RelatedTermCreateDTO RelatedTerm) : IValidatableRequest<Result<RelatedTermDTO>>;