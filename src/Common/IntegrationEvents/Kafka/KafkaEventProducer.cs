using System.Text.Json;
using Common.IntegrationEvents.Events;
using Confluent.Kafka;

namespace Common.IntegrationEvents.Kafka;

public class KafkaIntegrationEventProducer : IIntegrationEventProducer
{
    private readonly IProducer<string, string> _producer;
    public KafkaIntegrationEventProducer(string bootstrapServers)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers
        };
        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task SendEvent<T>(string topic, string key, T eventObject, CancellationToken cancellationToken) where T : IIntegrationEvent
    {
        var value = JsonSerializer.Serialize(eventObject);
        var message = new Message<string, string> { Key = key, Value = value };
        await _producer.ProduceAsync(topic, message, cancellationToken);
    }
}