using FluentResults;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Behavior;
using Streetcode.BLL.Services.CacheService;

namespace Streetcode.BLL.MediatR.Streetcode.Facts.Update
{
    public record UpdateFactCommand(FactUpdateCreateDto Fact) : IValidatableRequest<Result<FactDto>>, ICachibleCommandPostProcessor<Result<FactDto>>;
}
