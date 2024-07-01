using FluentResults;
using Streetcode.BLL.Behavior;
using Streetcode.BLL.DTO.Streetcode.TextContent.RelatedTerm;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Update;

public record UpdateRelatedTermCommand(RelatedTermDTO RelatedTerm) : IValidatableRequest<Result<RelatedTermDTO>>;