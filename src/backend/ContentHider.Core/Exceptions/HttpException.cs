using System;
using System.Net;

namespace ContentHider.Core.Exceptions;

public class HttpException : Exception
{
    public HttpStatusCode ErrorCode { get; private set; }

    public HttpException(Exception? ex, string message, HttpStatusCode code) : base(message, ex)
    {
        ErrorCode = code;
    }
}