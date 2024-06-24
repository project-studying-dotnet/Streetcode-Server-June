using FluentResults;
using MediatR;
using Streetcode.BLL.Behavior;

namespace Streetcode.BLL.MediatR.Streetcode.Text.DeleteById;

public record DeleteTextByIdCommand(int Id) : IValidatableRequest<Result<Unit>>;
