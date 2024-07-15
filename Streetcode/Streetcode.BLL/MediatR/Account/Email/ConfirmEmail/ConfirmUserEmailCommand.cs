using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Streetcode.BLL.Interfaces.Email;

namespace Streetcode.BLL.MediatR.Account.Email.ConfirmEmail
{
    public record ConfirmUserEmailCommand(string userId, string token)
        : IRequest<Result<string>>;
}
