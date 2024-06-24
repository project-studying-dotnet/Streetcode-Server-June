using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.Behavior;

namespace Streetcode.BLL.MediatR.Partners.Create
{
  public record CreatePartnerCommand(CreatePartnerDTO newPartner) : IValidatableRequest<Result<PartnerDTO>>;
}
