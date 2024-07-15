using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.MediatR.Account.Delete;
using Streetcode.BLL.MediatR.Account.Login;
using Streetcode.BLL.MediatR.Account.Logout;
using Streetcode.BLL.MediatR.Account.RefreshToken;
using Streetcode.BLL.MediatR.Account.Register;
using Streetcode.BLL.MediatR.Account.Email.ConfirmEmail;
using Streetcode.BLL.MediatR.Account.Email.SendEmail;
using Streetcode.BLL.MediatR.Account.RestorePassword;
using Streetcode.BLL.MediatR.Account.ChangePassword;
using Streetcode.BLL.MediatR.Account.LoginWithGoogle;

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
        public async Task<ActionResult<string>> RefreshTokens()
        {
            return HandleResult(await Mediator.Send(new RefreshTokensCommand()));
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

        [HttpPost]
        public async Task<IActionResult> LoginWithGoogle([FromBody] LoginWithGoogleDTO loginWithGoogle)
        {
            return HandleResult(await Mediator.Send(new LoginWithGoogleCommand(loginWithGoogle)));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            return HandleResult(await Mediator.Send(new DeleteUserCommand()));
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            return HandleResult(await Mediator.Send(new ConfirmUserEmailCommand(userId, token)));
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail([FromQuery] string email)
        {
            return HandleResult(await Mediator.Send(new SendVerificationEmailCommand(email)));
        }
        
        [HttpPost]
        public async Task<IActionResult> RestorePasswordRequest([FromBody] RestorePasswordRequestDto dto)
        {
            return HandleResult(await Mediator.Send(new RestorePasswordRequest(dto)));
        }
        
        [HttpPut]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO password)
        {
            return HandleResult(await Mediator.Send(new ChangePasswordCommand(password)));
        }

        [HttpPost]
        public async Task<IActionResult> RestorePassword([FromBody] RestorePasswordDto restorePasswordDto)
        {
            return HandleResult(await Mediator.Send(new RestorePasswordCommand(restorePasswordDto)));
        }
    }
}
