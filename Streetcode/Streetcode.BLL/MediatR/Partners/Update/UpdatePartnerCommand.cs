using FluentResults;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.Behavior;
using Streetcode.BLL.Services.CacheService;

namespace Streetcode.BLL.MediatR.Partners.Update
{
  public record UpdatePartnerCommand(CreatePartnerDTO Partner) : IValidatableRequest<Result<PartnerDTO>>, ICachibleCommandPostProcessor<Result<PartnerDTO>>;
}
