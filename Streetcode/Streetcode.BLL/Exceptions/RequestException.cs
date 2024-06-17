using System.Net;

namespace Streetcode.BLL.Exceptions;

public class RequestException : Exception
{
    public RequestException(string message, HttpStatusCode statusCode) : base(message)
    {
        StatusCode = statusCode;
    }

    public RequestException()
    {
    }

    public RequestException(string message) : base(message)
    {
    }

    public RequestException(string message, Exception inner) : base(message, inner)
    {
    }

    public HttpStatusCode StatusCode { get; }
}