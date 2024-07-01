using FluentResults;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.Services.CacheService;

namespace Streetcode.BLL.MediatR.Partners.GetByStreetcodeId;

public record GetPartnersByStreetcodeIdQuery(int StreetcodeId) : ICachibleQueryBehavior<Result<IEnumerable<PartnerDTO>>>
{
    public Result<IEnumerable<PartnerDTO>>? CachedResponse { get; set; }
}
