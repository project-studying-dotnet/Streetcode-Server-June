using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.Behavior;

namespace Streetcode.BLL.MediatR.Partners.Update
{
  public record UpdatePartnerCommand(CreatePartnerDTO Partner) : IValidatableRequest<Result<PartnerDTO>>;
}
