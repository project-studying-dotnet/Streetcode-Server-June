using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.Behavior;

namespace Streetcode.BLL.MediatR.Partners.Delete
{
    public record DeletePartnerCommand(int id) : IValidatableRequest<Result<PartnerDTO>>;
}
