using FluentResults;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.Services.CacheService;

namespace Streetcode.BLL.MediatR.Partners.GetById;

public record GetPartnerByIdQuery(int Id) : ICachibleQueryBehavior<Result<PartnerDTO>>
{
    public Result<PartnerDTO>? CachedResponse { get; set; }
}
