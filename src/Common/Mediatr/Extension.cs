using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Mediatr;

public static class Extension
{
    public static void AddMediatrWithPipelines(this IServiceCollection serviceCollection, Assembly assembly)
    {
        serviceCollection.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        serviceCollection.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
    }
}