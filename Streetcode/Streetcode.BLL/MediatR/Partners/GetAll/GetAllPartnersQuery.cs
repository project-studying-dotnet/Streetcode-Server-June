using FluentResults;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.Services.CacheService;

namespace Streetcode.BLL.MediatR.Partners.GetAll;

public record GetAllPartnersQuery : ICachibleQueryBehavior<Result<IEnumerable<PartnerDTO>>>
{
    public Result<IEnumerable<PartnerDTO>>? CachedResponse { get; set; }
}
