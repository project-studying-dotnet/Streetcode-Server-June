using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.Behavior;

namespace Streetcode.BLL.MediatR.Account.EmailVerification.SendEmail
{
    public record SendVerificationEmailCommand(string email)
       : IValidatableRequest<Result<string>>;
}
