using FluentResults;
using Streetcode.BLL.Behavior;
using Streetcode.BLL.DTO.Streetcode.TextContent.Term;

namespace Streetcode.BLL.MediatR.Streetcode.Term.Create;

public record CreateTermCommand(TermCreateDTO Term) : IValidatableRequest<Result<TermDTO>>;