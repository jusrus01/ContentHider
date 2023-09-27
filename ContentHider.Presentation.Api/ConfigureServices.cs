using ContentHider.Core.Dtos;
using ContentHider.Core.Exceptions;
using ContentHider.Core.Services;

namespace ContentHider.Presentation.Api;

public static class ConfigureServices
{
    public static void ConfigureExceptionHandler(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            try
            {
                await next();
            }
            catch (HttpException ex)
            {
                context.Response.StatusCode = (int)ex.ErrorCode;
                context.Response.Headers.Clear();

                await context.Response.WriteAsJsonAsync(GetErrorResponse(ex));
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.Headers.Clear();

                await context.Response.WriteAsJsonAsync(GetErrorResponse(ex));
            }
        });
    }

    private static object GetErrorResponse(Exception ex)
    {
        if (ex is HttpException httpException)
        {
            return new
            {
                httpException.Message, httpException.ErrorCode
            };
        }

        return new
        {
            ex.Message,
            ErrorCode = StatusCodes.Status500InternalServerError
        };
    }

    public static void ConfigureOrganizationEndPoints(this IEndpointRouteBuilder app)
    {
        app.MapPost(Constants.Routes.OrganizationRoute,
            async (CreateOrgDto org, CancellationToken token, IOrganizationService orgService)
                => Results.Ok(await orgService.CreateAsync(org, token).ConfigureAwait(false)));
    }
}