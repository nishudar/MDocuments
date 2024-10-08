using System.Diagnostics;
using System.Text.Json;
using Common.Abstracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Common.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (BusinessException ex)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Business rule validation",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest,
                Instance = context.Request.Path,
                Type = ex.GetType().ToString()
            };

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/problem+json";

            var problemDetailsJson = JsonSerializer.Serialize(problemDetails);
            await context.Response.WriteAsync(problemDetailsJson);
        }
        catch (Exception ex)
        {
            ex = ex.Demystify();
            var problemDetails = new ProblemDetails
            {
                Title = "Internal server error",
                Detail = ex.Message + ex.StackTrace,
                Status = StatusCodes.Status500InternalServerError,
                Instance = context.Request.Path
            };

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";

            var problemDetailsJson = JsonSerializer.Serialize(problemDetails);
            await context.Response.WriteAsync(problemDetailsJson);
        }
    }
}