//TODO: Finish consumption of the event on kafka to synchronize users between services

/*
using System.Text.Json;
using Common.IntegrationEvents.Events;
using Documents.Application.Commands;
using Documents.Domain.Entities;
using Documents.Domain.Events;
using MediatR;

namespace Documents.Infrastructure;

using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class KafkaConsumerBackgroundService : BackgroundService
{
    private readonly ILogger<KafkaConsumerBackgroundService> _logger;
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly IMediator _mediator;
    private readonly string _kafkaServer;
    
    public KafkaConsumerBackgroundService(ILogger<KafkaConsumerBackgroundService> logger, IMediator mediator, string kafkaServer)
    {
        _logger = logger;
        _kafkaServer = kafkaServer;
        _mediator = mediator;

        var config = new ConsumerConfig
        {
            GroupId = "user-events-group",
            BootstrapServers = _kafkaServer,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(IntegrationTopics.UsersTopic);

        return Task.Run(() =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(stoppingToken);
                    if (consumeResult != null)
                    {
                        HandleMessage(consumeResult);
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError("Error consuming message: {ErrorReason}", ex.Error);
                }
            }
        }, stoppingToken);
    }

    private void HandleMessage(ConsumeResult<Ignore, string> consumeResult)
    {
        try
        {
            var eventType = GetEventTypeFromHeaders(consumeResult.Message.Headers);

            if (eventType == null)
            {
                _logger.LogWarning("EventType header not found.");
                return;
            }

            switch (eventType)
            {
                case nameof(UserCreatedIntegrationEvent):
                    var userCreatedEvent = JsonSerializer.Deserialize<UserCreatedIntegrationEvent>(consumeResult.Message.Value);
                    HandleUserCreatedEvent(userCreatedEvent);
                    break;

                case nameof(UserUpdatedIntegrationEvent):
                    var userUpdatedEvent = JsonSerializer.Deserialize<UserUpdatedIntegrationEvent>(consumeResult.Message.Value);
                    HandleUserUpdatedEvent(userUpdatedEvent);
                    break;

                default:
                    _logger.LogWarning("Unrecognized event type: {EventType}", eventType);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error processing message: {ex.Message}");
        }
    }

    private string? GetEventTypeFromHeaders(Headers headers)
    {
        var header = headers?.FirstOrDefault(h => h.Key == "EventType");
        if (header != null)
        {
            return Encoding.UTF8.GetString(header.GetValueBytes());
        }

        return null;
    }

    private void HandleUserCreatedEvent(UserCreatedIntegrationEvent userCreatedEvent)
    {
        _mediator.Send(new BusinessUserAddedEvent
        {
            new BusinessUser()
            {
                Name = userCreatedEvent.Name,
                Id = userCreatedEvent.UserId
            }
        });
    }

    private void HandleUserUpdatedEvent(UserUpdatedIntegrationEvent userUpdatedEvent)
    {
        _logger.LogInformation($"User updated: {userUpdatedEvent.UserId}, Name: {userUpdatedEvent.Name}");
    }

    public override void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
        base.Dispose();
    }
}

*/