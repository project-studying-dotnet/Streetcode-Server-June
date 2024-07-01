using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Streetcode.TextContent.Term;
using Streetcode.BLL.MediatR.Streetcode.Term.Create;
using Streetcode.BLL.MediatR.Streetcode.Term.Delete;
using Streetcode.BLL.MediatR.Streetcode.Term.DeleteById;
using Streetcode.BLL.MediatR.Streetcode.Term.GetAll;
using Streetcode.BLL.MediatR.Streetcode.Term.GetById;
using Streetcode.BLL.MediatR.Streetcode.Term.Update;

namespace Streetcode.WebApi.Controllers.Streetcode.TextContent;

public class TermController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TermCreateDTO term)
    {
        return HandleResult(await Mediator.Send(new CreateTermCommand(term)));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return HandleResult(await Mediator.Send(new GetAllTermsQuery()));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new GetTermByIdQuery(id)));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] TermDTO term)
    {
        return HandleResult(await Mediator.Send(new UpdateTermCommand(term)));
    }

    [HttpDelete("{title}")]
    public async Task<IActionResult> Delete([FromRoute] string title)
    {
        return HandleResult(await Mediator.Send(new DeleteTermCommand(title)));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteById([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new DeleteTermByIdCommand(id)));
    }
}
