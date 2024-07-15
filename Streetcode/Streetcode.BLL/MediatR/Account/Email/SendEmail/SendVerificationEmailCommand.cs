using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Streetcode.BLL.Behavior;

namespace Streetcode.BLL.MediatR.Account.Email.SendEmail
{
    public record SendVerificationEmailCommand(string email)
       : IValidatableRequest<Result<string>>;
}
