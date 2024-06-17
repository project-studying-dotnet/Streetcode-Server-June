using System.Net;

namespace Streetcode.BLL.Exceptions;

public class RequestException : Exception
{
    public RequestException(HttpStatusCode statusCode)
    {
        StatusCode = statusCode;
    }

    public RequestException(string message, HttpStatusCode statusCode) : base(message)
    {
        StatusCode = statusCode;
    }

    public RequestException(string message, HttpStatusCode statusCode, Exception inner) : base(message, inner)
    {
        StatusCode = statusCode;
    }

    public HttpStatusCode StatusCode { get; }
}