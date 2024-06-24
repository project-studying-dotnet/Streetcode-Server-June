using FluentResults;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.BLL.Behavior;

namespace Streetcode.BLL.MediatR.Streetcode.Text.Create;

public record CreateTextCommand(TextCreateDTO TextCreate) : IValidatableRequest<Result<TextDTO>>;