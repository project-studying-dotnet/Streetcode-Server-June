using System.Net;
using Streetcode.BLL.Enums;

namespace Streetcode.BLL.Exceptions.Abstract;

public abstract class RequestException : Exception
{
    protected RequestException(string message, ErrorType errorType, HttpStatusCode statusCode) : base(message)
    {
        ErrorType = errorType;
        StatusCode = statusCode;
    }

    public ErrorType ErrorType { get; }
    public HttpStatusCode StatusCode { get; }
}