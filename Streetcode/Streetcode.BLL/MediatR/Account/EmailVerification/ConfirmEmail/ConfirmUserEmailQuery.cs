using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Streetcode.BLL.Interfaces.Email;

namespace Streetcode.BLL.MediatR.Account.EmailVerification.ConfirmEmail
{
    public record ConfirmUserEmailQuery(string userId, string token, UserManager<IdentityUser> userManager)
        : IRequest<Result<string>>;
}
