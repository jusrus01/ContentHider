using ContentHider.Core;
using ContentHider.Core.Dtos.Auth;
using ContentHider.Core.Dtos.Formats;
using ContentHider.Core.Dtos.Organizations;
using ContentHider.Core.Dtos.Rules;
using ContentHider.Core.Exceptions;
using ContentHider.Core.Services;
using Microsoft.AspNetCore.Authorization;

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
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
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
        app.MapGet(Constants.Routes.OrganizationPreviewRoute,
            async (CancellationToken token, IOrganizationService orgService)
                => Results.Ok(await orgService.GetAllPreviewAsync(token).ConfigureAwait(false)));

        app.MapPost(Constants.Routes.OrganizationRoute,
                async (CreateOrgDto org, CancellationToken token, IOrganizationService orgService) =>
                    Results.Created(new Uri("about:blank"),
                        await orgService.CreateAsync(org, token).ConfigureAwait(false)))
            .RequireAdminAuthorization();

        app.MapGet(Constants.Routes.OrganizationRoute,
                async (CancellationToken token, IOrganizationService orgService)
                    => Results.Ok(await orgService.GetAllAsync(token).ConfigureAwait(false)))
            .RequireUserOrAdminAuthorization();

        app.MapGet($"{Constants.Routes.OrganizationRoute}/{{id}}",
                async (string id, CancellationToken token, IOrganizationService orgService)
                    => Results.Ok(await orgService.GetByIdAsync(id, token).ConfigureAwait(false)))
            .RequireUserOrAdminAuthorization();

        app.MapDelete($"{Constants.Routes.OrganizationRoute}/{{id}}",
                async (string id, CancellationToken token, IOrganizationService orgService)
                    => Results.Ok(await orgService.DeleteAsync(id, token).ConfigureAwait(false)))
            .RequireAdminAuthorization();

        app.MapPut($"{Constants.Routes.OrganizationRoute}/{{id}}",
                async (string id, UpdateOrgDto org, CancellationToken token, IOrganizationService orgService)
                    => Results.Ok(await orgService.UpdateAsync(id, org, token).ConfigureAwait(false)))
            .RequireAdminAuthorization();
    }

    public static void ConfigureTextFormatEndPoints(this IEndpointRouteBuilder app)
    {
        app.MapPost(Constants.Routes.TextFormatRoute,
                async (string orgId, CreateFormatDto format, CancellationToken token, IFormatService formatService) =>
                    Results.Created(new Uri("about:blank"),
                        await formatService.CreateAsync(orgId, format, token).ConfigureAwait(false)))
            .RequireAdminAuthorization();

        app.MapPut($"{Constants.Routes.TextFormatRoute}/{{id}}",
                async (
                        string orgId,
                        string id,
                        UpdateFormatDto format,
                        CancellationToken token,
                        IFormatService formatService) =>
                    Results.Ok(await formatService.UpdateAsync(orgId, id, format, token).ConfigureAwait(false)))
            .RequireAdminAuthorization();

        app.MapGet(Constants.Routes.TextFormatRoute,
                async (string orgId, CancellationToken token, IFormatService formatService)
                    => Results.Ok(await formatService.GetAllAsync(orgId, token).ConfigureAwait(false)))
            .RequireUserOrAdminAuthorization();

        app.MapGet($"{Constants.Routes.TextFormatRoute}/{{id}}",
                async (string orgId, string id, CancellationToken token, IFormatService formatService)
                    => Results.Ok(await formatService.GetByIdAsync(orgId, id, token).ConfigureAwait(false)))
            .RequireUserOrAdminAuthorization();

        app.MapDelete($"{Constants.Routes.TextFormatRoute}/{{id}}",
                async (
                        string orgId,
                        string id,
                        CancellationToken token,
                        IFormatService formatService) =>
                    Results.Ok(await formatService.DeleteAsync(orgId, id, token).ConfigureAwait(false)))
            .RequireAdminAuthorization();
    }

    public static void ConfigureAuthEndPoints(this IEndpointRouteBuilder app)
    {
        app.MapPost(
            $"{Constants.Routes.AuthRoute}/register",
            async (RequestRegisterDto args, CancellationToken token, IAuthService authService) =>
                Results.Ok(await authService.RegisterAsync(args)));

        app.MapPost(
            $"{Constants.Routes.AuthRoute}/login",
            async (RequestTokenDto args, CancellationToken token, IAuthService authService) =>
                Results.Ok(await authService.GenerateTokenAsync(args)));

        app.MapPost(
            $"{Constants.Routes.AuthRoute}/refresh-login",
            async (RequestRefreshTokenDto args, CancellationToken token, IAuthService authService) =>
                Results.Ok(await authService.GenerateTokenAsync(args)));
    }

    public static void ConfigureRulesEndPoints(this IEndpointRouteBuilder app)
    {
        app.MapPost(Constants.Routes.RuleRoute,
                async (string orgId, string formatId, CreateRuleDto rule, CancellationToken token,
                        IRuleService ruleService) =>
                    Results.Created(new Uri("about:blank"),
                        await ruleService.CreateAsync(orgId, formatId, rule, token).ConfigureAwait(false)))
            .RequireAdminAuthorization();

        app.MapPost($"{Constants.Routes.RuleRoute}/apply",
                async (string orgId, string formatId, ApplyRuleDto rule, CancellationToken token,
                        IRuleService ruleService) =>
                    Results.Ok(await ruleService.ApplyAsync(orgId, formatId, rule.Text, token).ConfigureAwait(false)))
            .RequireUserOrAdminAuthorization();

        app.MapPut($"{Constants.Routes.RuleRoute}/{{id}}",
                async (
                        string orgId,
                        string formatId,
                        string id,
                        UpdateRuleDto rule,
                        CancellationToken token,
                        IRuleService ruleService) =>
                    Results.Ok(await ruleService.UpdateAsync(orgId, formatId, id, rule, token).ConfigureAwait(false)))
            .RequireAdminAuthorization();

        app.MapGet(Constants.Routes.RuleRoute,
                async (string orgId, string formatId, CancellationToken token, IRuleService ruleService)
                    => Results.Ok(await ruleService.GetAllAsync(orgId, formatId, token).ConfigureAwait(false)))
            .RequireUserOrAdminAuthorization();

        app.MapGet($"{Constants.Routes.RuleRoute}/{{id}}",
                async (string orgId, string formatId, string id, CancellationToken token, IRuleService ruleService)
                    => Results.Ok(await ruleService.GetByIdAsync(orgId, formatId, id, token).ConfigureAwait(false)))
            .RequireUserOrAdminAuthorization();

        app.MapDelete($"{Constants.Routes.RuleRoute}/{{id}}",
                async (
                        string orgId,
                        string formatId,
                        string id,
                        CancellationToken token,
                        IRuleService ruleService) =>
                    Results.Ok(await ruleService.DeleteAsync(orgId, formatId, id, token).ConfigureAwait(false)))
            .RequireAdminAuthorization();
    }

    private static TBuilder RequireAdminAuthorization<TBuilder>(this TBuilder builder)
        where TBuilder : IEndpointConventionBuilder
    {
        var authAttribute = new AuthorizeAttribute
        {
            Roles = Constants.Roles.Admin
        };

        return builder.RequireAuthorization(authAttribute);
    }

    public static TBuilder RequireUserOrAdminAuthorization<TBuilder>(this TBuilder builder)
        where TBuilder : IEndpointConventionBuilder
    {
        var authAttribute = new AuthorizeAttribute
        {
            Roles = $"{Constants.Roles.User}, {Constants.Roles.Admin}"
        };

        return builder.RequireAuthorization(authAttribute);
    }
}