using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Likes;
using Streetcode.BLL.MediatR.Likes.PushLike;
    
namespace Streetcode.WebApi.Controllers.Likes
{
    public class LikeController : BaseApiController
    {
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PushLike([FromBody] PushLikeDTO pushLike)
        {
            return HandleResult(await Mediator.Send(new PushLikeCommand(pushLike)));
        }
    }
}
