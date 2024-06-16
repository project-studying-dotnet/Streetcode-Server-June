using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;

namespace Streetcode.BLL.MediatR.Streetcode.Text.Create;

public record CreateTextCommand(TextCreateDTO TextCreate) : IRequest<Result<TextDTO>>
{
}