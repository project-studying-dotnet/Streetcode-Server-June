﻿using System.Net;

namespace Streetcode.BLL.Exceptions;

public class RequestException : Exception
{
    public RequestException(string message, HttpStatusCode statusCode) : base(message)
    {
        StatusCode = statusCode;
    }

    public HttpStatusCode StatusCode { get; }
}