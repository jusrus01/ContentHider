using System.Net;
using ContentHider.Core.Entities;
using ContentHider.Core.Enums;
using ContentHider.Core.Exceptions;
using ContentHider.Core.Repositories;

namespace ContentHider.Presentation.Api.Middlewares;

public class SimpleAuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public SimpleAuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    // ReSharper disable once UnusedMember.Global
    public async Task InvokeAsync(HttpContext context, IUnitOfWork uow)
    {
        if (!context.Request.Headers.TryGetValue("User-Id", out var userId))
        {
            throw new HttpException(null, "No user associated with the request", HttpStatusCode.Forbidden);
        }

        var user = (await uow.GetAsync<UserDao>(i => i.Id == userId, context.RequestAborted)
            ).SingleOrDefault();
        if (user == null)
        {
            throw new HttpException(null, "Could not find provided user", HttpStatusCode.Forbidden);
        }

        switch (context.Request.Method)
        {
            // allow everyone to use GET methods
            case "GET":
                await _next(context);
                break;
            case "POST" or "PUT" or "DELETE":
                EnsureUserAllowed(user);
                await _next(context);
                break;
        }
    }

    private static void EnsureUserAllowed(UserDao user)
    {
        if (user.Role != Roles.Admin)
        {
            throw new HttpException(null, "User cannot do this action", HttpStatusCode.Unauthorized);
        }
    }
}