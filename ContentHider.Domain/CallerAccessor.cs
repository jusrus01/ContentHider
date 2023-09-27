using System.Net;
using ContentHider.Core.Exceptions;
using ContentHider.Core.Services;
using Microsoft.AspNetCore.Http;

namespace ContentHider.Domain;

public class CallerAccessor : ICallerAccessor
{
    private readonly IHttpContextAccessor _accessor;

    public CallerAccessor(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public string UserId
    {
        get
        {
            if (!_accessor.HttpContext.Request.Headers.TryGetValue("User-Id", out var userId))
            {
                throw new HttpException(null, "No user associated with the request", HttpStatusCode.Forbidden);
            }

            return userId;
        }
    }
}