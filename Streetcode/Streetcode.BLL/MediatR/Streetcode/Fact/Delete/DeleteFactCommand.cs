using FluentResults;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Behavior;
using Streetcode.BLL.Services.CacheService;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Delete
{
    public record DeleteFactCommand(int Id) : IValidatableRequest<Result<FactDto>>, ICachibleCommandPostProcessor<Result<FactDto>>;
}
