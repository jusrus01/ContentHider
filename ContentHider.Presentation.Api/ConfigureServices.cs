using ContentHider.Core.Dtos;
using ContentHider.Core.Dtos.Formats;
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
            async (CreateOrgDto org, CancellationToken token, IOrganizationService orgService) =>
                Results.Ok(await orgService.CreateAsync(org, token).ConfigureAwait(false)));

        app.MapGet(Constants.Routes.OrganizationRoute,
            async (CancellationToken token, IOrganizationService orgService)
                => Results.Ok(await orgService.GetAllAsync(token).ConfigureAwait(false)));

        app.MapGet($"{Constants.Routes.OrganizationRoute}/{{id}}",
            async (string id, CancellationToken token, IOrganizationService orgService)
                => Results.Ok(await orgService.GetByIdAsync(id, token).ConfigureAwait(false)));

        app.MapDelete($"{Constants.Routes.OrganizationRoute}/{{id}}",
            async (string id, CancellationToken token, IOrganizationService orgService)
                => Results.Ok(await orgService.DeleteAsync(id, token).ConfigureAwait(false)));

        app.MapPut($"{Constants.Routes.OrganizationRoute}/{{id}}",
            async (string id, UpdateOrgDto org, CancellationToken token, IOrganizationService orgService)
                => Results.Ok(await orgService.UpdateAsync(id, org, token).ConfigureAwait(false)));
    }

    public static void ConfigureTextFormatEndPoints(this IEndpointRouteBuilder app)
    {
        app.MapPost(Constants.Routes.TextFormatRoute,
            async (string orgId, OrgCreateFormatDto format, CancellationToken token, IFormatService formatService) =>
                Results.Ok(await formatService.CreateAsync(orgId, format, token).ConfigureAwait(false)));
    }

    public static void ConfigureRulesEndPoints(this IEndpointRouteBuilder app)
    {
    }
}