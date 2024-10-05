using System.Text;
using System.Text.Json;
using Common.IntegrationEvents.Events;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Common.IntegrationEvents.Kafka;

public class KafkaIntegrationEventProducer(string bootstrapServers, ILogger<KafkaIntegrationEventProducer> logger) : IIntegrationEventProducer
{
    private readonly ProducerConfig _config = new()
    {
        BootstrapServers = bootstrapServers
    };

    private sealed class IntegrationEventSerializer<T> : Confluent.Kafka.ISerializer<T> where T: IIntegrationEvent
    {
        public byte[] Serialize(T? data, SerializationContext context)
        {
            return object.Equals(data, default(T)) ? [] : JsonSerializer.SerializeToUtf8Bytes(data, data.GetType());
        }
    }

    
    public async Task Publish<T>(T eventObject, string key, string topic, CancellationToken cancellationToken) where T : IIntegrationEvent
    {
        var producer = new ProducerBuilder<string, T>(_config).Build();
        var message = new Message<string, T>
        {
            Key = key,
            Value = eventObject,
        };
		
        await producer.ProduceAsync(topic, message, cancellationToken);
    }
    
    public async Task Publish<T>(
        T? eventObject, 
        string key,
        string eventType,
        string topic,
        CancellationToken cancellationToken) 
        where T : IIntegrationEvent
    {
        try
        {
            var producer = new ProducerBuilder<string, T?>(_config)
                .SetValueSerializer(new IntegrationEventSerializer<T?>())
                .Build();
            var message = new Message<string, T?>
            {
                Key = key,
                Value = eventObject,
                Headers = new Headers
                {
                    {"eventType", Encoding.UTF8.GetBytes(typeof(T).Name)}
                }
            };
            await producer.ProduceAsync(topic, message, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{ExMessage} + {InnerExceptionMessage}", ex.Message, ex.InnerException?.Message);
        }
    }
}