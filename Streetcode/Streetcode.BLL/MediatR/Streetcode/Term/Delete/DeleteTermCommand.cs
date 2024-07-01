using FluentResults;
using Streetcode.BLL.Behavior;
using Streetcode.BLL.DTO.Streetcode.TextContent.Term;

namespace Streetcode.BLL.MediatR.Streetcode.Term.Delete;

public record DeleteTermCommand(string Title) : IValidatableRequest<Result<TermDTO>>;