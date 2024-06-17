using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Create;

public record CreateFactCommand(FactDto newFact) : IRequest<Result<FactDto>>;
