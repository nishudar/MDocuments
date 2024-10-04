using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Common.DomainEvents;

public static class Extension
{
    public static void AddDomainEventHandlers(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssemblies(Assembly.GetExecutingAssembly())
            .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.AddTransient<IDomainEventDispatcher, DomainEventDispatcher>();
    }
}