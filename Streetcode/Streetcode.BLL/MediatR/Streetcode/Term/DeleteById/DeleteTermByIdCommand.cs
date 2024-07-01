using FluentResults;
using Streetcode.BLL.Behavior;
using Streetcode.BLL.DTO.Streetcode.TextContent.Term;

namespace Streetcode.BLL.MediatR.Streetcode.Term.DeleteById;

public record DeleteTermByIdCommand(int Id) : IValidatableRequest<Result<TermDTO>>;