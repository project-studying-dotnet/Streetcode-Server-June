using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Org.BouncyCastle.Asn1.Ocsp;
using Streetcode.BLL.Interfaces.Email;
using Streetcode.DAL.Entities.AdditionalContent.Email;
using Streetcode.DAL.Entities.Users;
using System.Net.Http;

namespace Streetcode.BLL.Util.Account
{
    public class SendVerificationEmail
    {
        private const string ACTION = "ConfirmEmail";
        private const string CONTROLLER = "Account";
        private const string SUBJECT = "Confirm your email";
        private const string FROM = "Streetcode";

        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailSender;
        private readonly LinkGenerator _linkGenerator;
        private HttpContext _httpContext = null!;

        public SendVerificationEmail(UserManager<User> userManager, IEmailService emailSender, LinkGenerator linkGenerator)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _linkGenerator = linkGenerator;
            
        }

        public async Task SendVerification(string email, HttpContext httpContext)
        {
            _httpContext = httpContext;

            string url = await CreateUrl(email);

            await SendEmail(email, url);
        }

        public async Task<string> CreateUrl(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var url = _linkGenerator.GetPathByAction(
                    _httpContext, ACTION, CONTROLLER, new { userId = user.Id, token = token });
                var baseUrl = $"{_httpContext.Request.Scheme}://{_httpContext.Request.Host}";
                var fullUrl = baseUrl + url;

                return fullUrl!;
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
                    new Message(new List<string> { email }, FROM, SUBJECT, url!));
        }
    }
}
