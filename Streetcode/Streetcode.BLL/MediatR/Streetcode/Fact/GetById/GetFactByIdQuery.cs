using FluentResults;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.Services.CacheService;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.GetById;

public record GetFactByIdQuery(int Id) : ICachibleQueryBehavior<Result<FactDto>>
{
    public Result<FactDto>? CachedResponse { get; set; }
}
