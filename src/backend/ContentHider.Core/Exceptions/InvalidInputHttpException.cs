using System.Net;

namespace ContentHider.Core.Exceptions;

public class InvalidInputHttpException : HttpException
{
    private const HttpStatusCode Code = HttpStatusCode.BadRequest;

    public InvalidInputHttpException(Exception? ex, string message, HttpStatusCode code = Code) : base(ex, message, code)
    {
    }
}