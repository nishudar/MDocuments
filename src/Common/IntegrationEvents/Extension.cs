using Common.IntegrationEvents.Kafka;
using Microsoft.Extensions.DependencyInjection;

namespace Common.IntegrationEvents;

public static class Extensions
{
    public static void AddKafkaIntegrationEvents(this IServiceCollection services, string serverUrl) 
        => services.AddSingleton<IIntegrationEventProducer>(_ => new KafkaIntegrationEventProducer(serverUrl));
}