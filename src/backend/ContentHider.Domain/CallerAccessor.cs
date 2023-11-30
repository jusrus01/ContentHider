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
            var exception = new HttpException(null, "No user associated with the request", HttpStatusCode.Forbidden);

            if (_accessor.HttpContext.User == null)
            {
                throw exception;
            }

            if (_accessor.HttpContext.User.Claims == null)
            {
                throw exception;
            }

            var id = _accessor.HttpContext.User.Claims.SingleOrDefault(p => p.Type == "uid");
            if (id == null)
            {
                throw exception;
            }

            return id.Value;
        }
    }
}