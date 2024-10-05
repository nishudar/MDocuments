using System.Text.Json;
using Common.IntegrationEvents.Events;
using Confluent.Kafka;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace NotificationHub.Consumer;

public class KafkaConsumerService(
    IHubContext<Notifications.NotificationHub> hubContext,
    ILogger<KafkaConsumerService> logger,
    IOptions<KafkaConsumerConfiguration> options)
    : BackgroundService
{
    private readonly KafkaConsumerConfiguration _configuration = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            GroupId = _configuration.Group,
            BootstrapServers = _configuration.Server,
            AutoOffsetReset = AutoOffsetReset.Latest
        };
        
        //workaround to aviod logging error on startup(topic not created yet)
        await Task.Delay(3000, stoppingToken);
        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe(IntegrationTopics.DocumentsTopic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(stoppingToken);
                    var message = consumeResult?.Message?.Value;

                    if (message is not null)
                    {
                        await hubContext.Clients.All.SendAsync("ReceiveMessage", message,
                            cancellationToken: stoppingToken);
                        logger.LogInformation("Message broadcasted on signalR: {Message}", JsonSerializer.Serialize(message, options: new () {WriteIndented = false}));
                    }
                }
                catch (ConsumeException e)
                {
                    logger.LogError(e, "Kafka error: {ErrorReason}", e.Error.Reason);
                    await Task.Delay(3000, stoppingToken);
                }
            }
        }
        catch (OperationCanceledException)
        {
            //ignore
        }
        finally
        {
            consumer.Close();
        }
    }
}