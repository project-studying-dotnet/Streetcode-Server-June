using FluentResults;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.Behavior;
using Streetcode.BLL.Services.CacheService;

namespace Streetcode.BLL.MediatR.Partners.Delete
{
    public record DeletePartnerCommand(int id) : IValidatableRequest<Result<PartnerDTO>>, ICachibleCommandPostProcessor<Result<PartnerDTO>>;
}
