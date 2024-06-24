using FluentResults;
using Streetcode.BLL.Behavior;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;

namespace Streetcode.BLL.MediatR.Streetcode.Text.Update;

public record UpdateTextCommand(int Id, TextCreateDTO TextUpdate) : IValidatableRequest<Result<TextDTO>>;
