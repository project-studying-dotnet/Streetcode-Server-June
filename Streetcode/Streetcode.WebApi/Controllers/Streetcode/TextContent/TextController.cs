using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.BLL.MediatR.Streetcode.Text.Create;
using Streetcode.BLL.MediatR.Streetcode.Text.DeleteById;
using Streetcode.BLL.MediatR.Streetcode.Text.GetAll;
using Streetcode.BLL.MediatR.Streetcode.Text.GetById;
using Streetcode.BLL.MediatR.Streetcode.Text.GetByStreetcodeId;
using Streetcode.BLL.MediatR.Streetcode.Text.GetParsed;
using Streetcode.BLL.MediatR.Streetcode.Text.Update;

namespace Streetcode.WebApi.Controllers.Streetcode.TextContent;

public class TextController : BaseApiController
{
    [HttpPost]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> Create([FromBody] TextCreateDTO newText)
    {
        return HandleResult(await Mediator.Send(new CreateTextCommand(newText)));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(await Mediator.Send(new GetAllTextsQuery()));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new GetTextByIdQuery(id)));
    }

    [HttpGet("{streetcodeId:int}")]
    public async Task<IActionResult> GetByStreetcodeId([FromRoute] int streetcodeId)
    {
        return HandleResult(await Mediator.Send(new GetTextByStreetcodeIdQuery(streetcodeId)));
    }

    [HttpGet]
    public async Task<IActionResult> GetParsedText([FromQuery] string text)
    {
        return HandleResult(await Mediator.Send(new GetParsedTextForAdminPreviewCommand(text)));
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> UpdateById([FromRoute] int id, [FromBody] TextCreateDTO update)
    {
        return HandleResult(await Mediator.Send(new UpdateTextCommand(id, update)));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> DeleteById([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new DeleteTextByIdCommand(id)));
    }
}