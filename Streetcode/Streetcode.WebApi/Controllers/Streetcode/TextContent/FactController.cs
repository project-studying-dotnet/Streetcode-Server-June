using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Partners;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.MediatR.Streetcode.Fact.Delete;
using Streetcode.BLL.MediatR.Partners.Create;
using Streetcode.BLL.MediatR.Streetcode.Fact.Create;
using Streetcode.BLL.MediatR.Streetcode.Fact.GetAll;
using Streetcode.BLL.MediatR.Streetcode.Fact.GetById;
using Streetcode.BLL.MediatR.Streetcode.Fact.GetByStreetcodeId;
using Streetcode.BLL.MediatR.Streetcode.Fact.Reorder;
using Streetcode.BLL.MediatR.Streetcode.Facts.Update;
using Microsoft.AspNetCore.Authorization;

namespace Streetcode.WebApi.Controllers.Streetcode.TextContent;

public class FactController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(await Mediator.Send(new GetAllFactsQuery()));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new GetFactByIdQuery(id)));
    }

    [HttpGet("{streetcodeId:int}")]
    public async Task<IActionResult> GetByStreetcodeId([FromRoute] int streetcodeId)
    {
        return HandleResult(await Mediator.Send(new GetFactByStreetcodeIdQuery(streetcodeId)));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new DeleteFactCommand(id)));
    }

    [HttpPatch("{streetcodeId:int}")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> UpdateFactPositions([FromBody] IEnumerable<FactUpdatePositionDto> facts, [FromRoute] int streetcodeId)
    {
        return HandleResult(await Mediator.Send(new ReorderFactsCommand(facts, streetcodeId)));
    }

    [HttpPost]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> Create([FromBody] FactDto fact)
    {
        return HandleResult(await Mediator.Send(new CreateFactCommand(fact)));
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> Update([FromBody] FactUpdateCreateDto relatedTerm)
    {
        return HandleResult(await Mediator.Send(new UpdateFactCommand(relatedTerm)));
    }
}
