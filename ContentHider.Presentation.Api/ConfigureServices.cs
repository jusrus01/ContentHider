using System;
using System.Threading.Tasks;
using ContentHider.Core.Dtos;
using ContentHider.Core.Entities;
using ContentHider.Core.Exceptions;
using ContentHider.Core.Services;
using ContentHider.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ContentHider.Presentation.Api;

public static class ConfigureServices
{
    private const string OrganizationRoute = "/org";
    
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
                Message = httpException.Message,
                ErrorCode = httpException.ErrorCode
            };
        }
            
        return new
        {
            Message = ex.Message,
            ErrorCode = StatusCodes.Status500InternalServerError
        };
    }
    
    public static void ConfigureOrganizationEndPoints(this IEndpointRouteBuilder app)
    {
        async Task<IResult> ExecuteAsync(CreateOrgDto org, DelayedFunction executor)
        {
            var result = await executor
                .Begin(org)   
                .Validate<CreateOrgDto>(dto => !string.IsNullOrWhiteSpace(dto.Title))
                .Validate<CreateOrgDto>(dto => !string.IsNullOrWhiteSpace(dto.Description))
                .Map<CreateOrgDto, OrganizationDao>(dto => new OrganizationDao { Title = dto.Title })
                .ExecuteAsync<OrganizationDao>()
                .ConfigureAwait(false);
            
            return result.FromResult();
        }

        app.MapPost(OrganizationRoute, ExecuteAsync);

        app.MapPost($"{OrganizationRoute}/create",
            async (CreateOrgDto org, CancellationToken token, IOrganizationService orgService)
                => Results.Ok(await orgService.CreateAsync(org, token).ConfigureAwait(false)));
    }

    private static IResult FromResult(this bool result)
    {
        if (result)
        {
            return Results.Ok();
        }

        return Results.BadRequest();
    }
    
    private static IResult FromResult(this object result)
    {
        return Results.Ok(result);
    }
}