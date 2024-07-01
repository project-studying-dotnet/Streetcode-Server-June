using FluentResults;
using Streetcode.BLL.Behavior;
using Streetcode.BLL.DTO.Streetcode.TextContent.RelatedTerm;

namespace Streetcode.BLL.MediatR.Streetcode.RelatedTerm.Delete;

public record DeleteRelatedTermCommand(string Word) : IValidatableRequest<Result<RelatedTermDTO>>;