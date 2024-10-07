
using System.Text.Json;
using Common.IntegrationEvents.Events;
using Documents.Domain.Entities;
using Documents.Domain.Events;
using MediatR;
using Confluent.Kafka;
using System.Text;
namespace Documents.Infrastructure;

internal sealed class KafkaConsumerBackgroundService : BackgroundService
{
    private readonly ILogger<KafkaConsumerBackgroundService> _logger;
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly IMediator _mediator;

    public KafkaConsumerBackgroundService(ILogger<KafkaConsumerBackgroundService> logger, IMediator mediator, string kafkaServer)
    {
        _logger = logger;
        _mediator = mediator;

        var config = new ConsumerConfig
        {
            GroupId = "user-events-group",
            BootstrapServers = kafkaServer,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(IntegrationTopics.UsersTopic);
        return Task.Run(() =>
        {
            //workaround to aviod logging error on startup(topic not created yet)
            Task.Delay(3000, stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(stoppingToken);
                    if (consumeResult is not null)
                        HandleMessage(consumeResult);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming message: {ErrorReason}", ex?.Error);
                }
            }
        }, stoppingToken);
    }

    private void HandleMessage(ConsumeResult<Ignore, string> consumeResult)
    {
        try
        {
            var eventType = GetEventTypeFromHeaders(consumeResult.Message.Headers);

            if (eventType is null)
            {
                _logger.LogWarning("EventType header not found");
                return;
            }
            switch (eventType)
            {
                case nameof(UserCreatedIntegrationEvent):
                    var userCreatedEvent = JsonSerializer.Deserialize<UserCreatedIntegrationEvent>(consumeResult.Message.Value);
                    HandleUserCreatedEvent(userCreatedEvent!);
                    break;

                case nameof(UserUpdatedIntegrationEvent):
                    var userUpdatedEvent = JsonSerializer.Deserialize<UserUpdatedIntegrationEvent>(consumeResult.Message.Value);
                    HandleUserUpdatedEvent(userUpdatedEvent!);
                    break;

                default:
                    _logger.LogWarning("Unrecognized event type: {EventType}", eventType);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message: {ExMessage}", ex.Message);
        }
    }

    private static string? GetEventTypeFromHeaders(Headers headers)
    {
        var header = headers.FirstOrDefault(h => h.Key == "EventType");
        return header != null ? Encoding.UTF8.GetString(header.GetValueBytes()) : null;
    }

    private void HandleUserCreatedEvent(UserCreatedIntegrationEvent userCreatedEvent)
    {
        _mediator.Send(new BusinessUserAddedEvent
        {
            User = new BusinessUser
            {
                Name = userCreatedEvent.Name,
                Id = userCreatedEvent.UserId
            }
        });
    }

    private void HandleUserUpdatedEvent(UserUpdatedIntegrationEvent userUpdatedEvent)
    {
        _mediator.Send(new BusinessUserUpdatedEvent
        {
            User = new BusinessUser
            {
                Name = userUpdatedEvent.Name,
                Id = userUpdatedEvent.UserId
            }
        });
    }

    public override void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
        base.Dispose();
    }
}
