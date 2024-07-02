using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.MediatR.Account.EmailVerification.ConfirmEmail;
using Streetcode.BLL.MediatR.Account.Login;
using Streetcode.BLL.MediatR.Account.RefreshTokens;
using Streetcode.BLL.MediatR.Account.Logout;
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
        
        [HttpPost]
        public async Task<ActionResult<string>> RefreshTokens(TokenResponseDTO response)
        {
            return HandleResult(await Mediator.Send(new RefreshTokensCommand(response)));
        }
        
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            return HandleResult(await Mediator.Send(new LogoutUserCommand()));
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO loginUser)
        {
            return HandleResult(await Mediator.Send(new LoginUserCommand(loginUser)));
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
