using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Common.IntegrationEvents.Infrastructure;

public static class Extensions
{
    public static void AddKafkaIntegrationEvents(this IServiceCollection services, string serverUrl)
    {
        services.AddSingleton<IIntegrationEventProducer>(_ => new KafkaIntegrationEventProducer(serverUrl));
        services.AddTransient(typeof(INotificationHandler<>), typeof(IntegrationEventNotificationHandler<>));
    }
}