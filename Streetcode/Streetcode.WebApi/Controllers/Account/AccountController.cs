using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.MediatR.Account.Register;
using Microsoft.AspNetCore.Identity;
using Streetcode.BLL.Interfaces.Email;
using Streetcode.DAL.Entities.AdditionalContent.Email;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetAllCatalog;
using Streetcode.BLL.MediatR.Account.EmailVerification.ConfirmEmail;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using Streetcode.BLL.MediatR.Account.EmailVerification.SendVerificationEmail;

namespace Streetcode.WebApi.Controllers.Account
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailService _emailSender;

        public AccountController(UserManager<IdentityUser> userManager, IEmailService emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO newUser)
        {
            return HandleResult(await Mediator.Send(new RegisterUserCommand(newUser)));
        }

        [HttpPost]
        public async Task<IActionResult> SendVerificationEmail([FromBody] UserRegisterDTO model)
        {
            return HandleResult(await Mediator.Send(new SendVerificationEmailQuery(model, _userManager, _emailSender, this)));
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            return HandleResult(await Mediator.Send(new ConfirmUserEmailQuery(userId, token, _userManager)));
        }
    }
}
