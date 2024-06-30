using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Interfaces.Email;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.AdditionalContent.Email;
using System;

namespace Streetcode.BLL.Util.Account
{
    public class SendVerificationEmail
    {
        private const string ACTION = "ConfirmEmail";
        private const string CONTROLLER = "Account";
        private const string FROM = "Streetcode";
        private const string SUBJECT = "Confirm your email";

        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailService _emailSender;
        private readonly IUrlHelper _urlHelper;

        public SendVerificationEmail(UserManager<IdentityUser> userManager, IEmailService emailSender, IUrlHelper urlHelper)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _urlHelper = urlHelper;
        }

        public async void SendVerification(string email)
        {
            string url = await CreateUrl(email);
 
            await SendEmail(email, url);
        }

        public async Task<string> CreateUrl(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var url = _urlHelper.Action(
                    ACTION, CONTROLLER, new { userId = user.Id, token = token });

                return url!;
            }
            else
            {
                throw new Exception("");
            }
        }

        public async Task SendEmail(string email, string url)
        {
            await _emailSender
                    .SendEmailAsync(
                    new Message(new List<string> { email }, FROM, SUBJECT, url!));
        }
    }
}
