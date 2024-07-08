using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.MediatR.Partners.Create;
using Streetcode.BLL.MediatR.Partners.GetAll;
using Streetcode.BLL.MediatR.Partners.GetAllPartnerShort;
using Streetcode.BLL.MediatR.Partners.GetById;
using Streetcode.BLL.MediatR.Partners.GetByStreetcodeId;

namespace Streetcode.WebApi.Controllers.Partners;

public class PartnersController : BaseApiController
{
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(await Mediator.Send(new GetAllPartnersQuery()));
    }

    [Authorize(Roles = "User")]
    [HttpGet]
    public async Task<IActionResult> GetAllShort()
    {
        return HandleResult(await Mediator.Send(new GetAllPartnersShortQuery()));

    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new GetPartnerByIdQuery(id)));
    }

    [HttpGet("{streetcodeId:int}")]
    public async Task<IActionResult> GetByStreetcodeId([FromRoute] int streetcodeId)
    {
        return HandleResult(await Mediator.Send(new GetPartnersByStreetcodeIdQuery(streetcodeId)));
    }

    [HttpPost]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> Create([FromBody] CreatePartnerDTO partner)
    {
        return HandleResult(await Mediator.Send(new CreatePartnerCommand(partner)));
    }

    [HttpPut]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> Update([FromBody] CreatePartnerDTO partner)
    {
        return HandleResult(await Mediator.Send(new BLL.MediatR.Partners.Update.UpdatePartnerCommand(partner)));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new BLL.MediatR.Partners.Delete.DeletePartnerCommand(id)));
    }
}
