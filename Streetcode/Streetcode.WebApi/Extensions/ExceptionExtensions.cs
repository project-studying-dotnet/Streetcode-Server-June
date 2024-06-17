using System.Net;
using Streetcode.BLL.Exceptions;

namespace Streetcode.WebApi.Extensions;

public static class ExceptionExtensions
{
    public static ErrorDetailsDto GetErrorDetailsAndStatusCode(this Exception exception)
    {
        return exception switch
        {
            RequestException e => new ErrorDetailsDto(e.Message, e.StatusCode),
            _ => new ErrorDetailsDto(exception.Message, HttpStatusCode.InternalServerError)
        };
    }
}