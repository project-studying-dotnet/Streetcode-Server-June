﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Streetcode.BLL.Interfaces.Email;
using Streetcode.DAL.Entities.AdditionalContent.Email;

namespace Streetcode.BLL.Util.Account
{
    public class SendVerificationEmail
    {
        private const string ACTION = "ConfirmEmail";
        private const string CONTROLLER = "Account";
        private const string SUBJECT = "Confirm your email";

        private readonly string _from;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailService _emailSender;
        private readonly IUrlHelper _urlHelper;

        public SendVerificationEmail(UserManager<IdentityUser> userManager, IEmailService emailSender, IUrlHelper urlHelper, IConfiguration configuration)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _urlHelper = urlHelper;
            _from = configuration.GetSection("EmailConfiguration").GetSection("From").ToString() !;
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
                throw new Exception("User not found");
            }
        }

        public async Task SendEmail(string email, string url)
        {
            await _emailSender
                    .SendEmailAsync(
                    new Message(new List<string> { email }, _from, SUBJECT, url!));
        }
    }
}
