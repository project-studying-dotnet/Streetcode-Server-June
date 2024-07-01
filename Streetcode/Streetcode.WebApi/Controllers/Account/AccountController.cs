using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.MediatR.Account.Login;
using Streetcode.BLL.MediatR.Account.RefreshTokens;
using Streetcode.BLL.MediatR.Account.Logout;
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
    }
}
