using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.MediatR.Account.Register;
using Microsoft.AspNetCore.Identity;
using Streetcode.BLL.Interfaces.Email;
using Streetcode.DAL.Entities.AdditionalContent.Email;
using FluentResults;
using Org.BouncyCastle.Asn1.Ocsp;
using Streetcode.BLL.Resources;
using Streetcode.BLL.DTO.Streetcode;
using System.Net.Mail;

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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new IdentityUser { Email = model.Email, UserName = model.FirstName };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);

                await SendEmailAsync(model.Email, "Confirm your email", confirmationLink!);

                return Ok("The message was sent to the email");
            }

            return StatusCode(500, "Error when sending an email");
        }

        public async Task SendEmailAsync(string email, string subject, string body)
        {
            string from = "Streetcode";
            var message = new Message(new List<string> { email }, from, subject, body);

            await _emailSender.SendEmailAsync(message);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return Ok("userId or token is empty");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return Ok("Email is confirmed");
            }

            return StatusCode(500, "Email is not confirmed");
        }
    }
}
