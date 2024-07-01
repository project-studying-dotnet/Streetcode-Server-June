using FluentResults;
using Streetcode.BLL.Behavior;
using Streetcode.BLL.DTO.Streetcode.TextContent.Term;

namespace Streetcode.BLL.MediatR.Streetcode.Term.Update;

public record UpdateTermCommand(TermDTO Term) : IValidatableRequest<Result<TermDTO>>;