using System.Net;
using Streetcode.BLL.Enums;
using Streetcode.BLL.Exceptions.Abstract;

namespace Streetcode.BLL.Exceptions;

public sealed class EmailAlreadyRegisteredException : RequestException
{
    public EmailAlreadyRegisteredException() : base(
        "Email is already registered. Try another one",
        ErrorType.InvalidEmail,
        HttpStatusCode.BadRequest)
    {
    }
}