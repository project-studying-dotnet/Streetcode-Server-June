using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Interfaces.Email;
using Microsoft.AspNetCore.Mvc;

namespace Streetcode.BLL.MediatR.Account.EmailVerification.SendVerificationEmail
{
    public record SendVerificationEmailQuery
        (UserRegisterDTO model, UserManager<IdentityUser> userManager, IEmailService emailSender, ControllerBase controller)
        : IRequest<Result<string>>;
}
