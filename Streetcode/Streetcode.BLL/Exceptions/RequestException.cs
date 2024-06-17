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

    public RequestException(string message, Exception inner, HttpStatusCode statusCode) : base(message, inner)
    {
    }

    public HttpStatusCode StatusCode { get; }
}