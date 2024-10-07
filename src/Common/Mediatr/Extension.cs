using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Mediatr;

public static class Extension
{
    public static void AddMediatrWithPipelines(this IServiceCollection services, Assembly assembly)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestLoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(NotificationLoggingBehavior<,>));
    }
}