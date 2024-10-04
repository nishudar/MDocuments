using Microsoft.AspNetCore.Builder;

namespace Common.Middleware;

public static class Extension
{
    public static void UseCommonMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}