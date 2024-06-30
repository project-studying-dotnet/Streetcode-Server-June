using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.MediatR.Account.EmailVerification.ConfirmEmail;
using Streetcode.BLL.MediatR.Account.Register;

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
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            return HandleResult(await Mediator.Send(new ConfirmUserEmailCommand(userId, token)));
        }
    }
}
