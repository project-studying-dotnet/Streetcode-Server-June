using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.MediatR.Account.EmailVerification.ConfirmEmail;
using Streetcode.BLL.MediatR.Account.Register;
using Streetcode.BLL.MediatR.Account.EmailVerification.SendEmail;

namespace Streetcode.WebApi.Controllers.Account
{
    public class AccountController : BaseApiController
    {
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO newUser)
        {
            return HandleResult(await Mediator.Send(new RegisterUserCommand(newUser)));
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail([FromRoute]string userId, [FromRoute]string token)
        {
            return HandleResult(await Mediator.Send(new ConfirmUserEmailCommand(userId, token)));
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail([FromQuery] string email)
        {
            return HandleResult(await Mediator.Send(new SendVerificationEmailCommand(email)));
        }
    }
}
