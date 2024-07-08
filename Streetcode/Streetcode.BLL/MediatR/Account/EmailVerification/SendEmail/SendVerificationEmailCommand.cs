using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.Behavior;

namespace Streetcode.BLL.MediatR.Account.EmailVerification.SendEmail
{
    public record SendVerificationEmailCommand(string email, HttpContext httpContext)
       : IValidatableRequest<Result<string>>;
}
