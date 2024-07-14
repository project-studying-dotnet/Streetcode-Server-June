using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Comment;
using Streetcode.BLL.MediatR.Replies;
using Streetcode.BLL.MediatR.Replies.Delete;
using Streetcode.BLL.MediatR.Streetcode.Fact.Delete;

namespace Streetcode.WebApi.Controllers.Reply;

public class ReplyController : BaseApiController
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ReplyCreateDTO reply)
    {
        return HandleResult(await Mediator.Send(new CreateReplyCommand(reply)));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        return HandleResult(await Mediator.Send(new DeleteReplyCommand(id)));
    }
}