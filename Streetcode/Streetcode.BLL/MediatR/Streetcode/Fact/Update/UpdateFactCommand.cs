using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;

namespace Streetcode.BLL.MediatR.Streetcode.Facts.Update
{
    public record UpdateFactCommand(FactDto Fact) : IRequest<Result<FactDto>>;
}
