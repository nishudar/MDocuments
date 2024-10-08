﻿using System.Text;
using System.Text.Json;
using Common.IntegrationEvents.Events;
using Confluent.Kafka;
using Documents.Domain.Entities;
using Documents.Domain.Events;
using MediatR;

namespace Documents.Application.IntegrationEventsHandler;

internal sealed class IntegrationEventsHandlerService : BackgroundService
{
    private readonly ILogger<IntegrationEventsHandlerService> _logger;
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly IMediator _mediator;

    public IntegrationEventsHandlerService(ILogger<IntegrationEventsHandlerService> logger, IMediator mediator, string kafkaServer)
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

    private void HandleMessage(ConsumeResult<Ignore, string> message)
    {
        try
        {
            var eventType = GetEventTypeFromHeaders(message.Message.Headers);

            if (eventType is null)
            {
                _logger.LogWarning("EventType header not found");
                return;
            }
            switch (eventType)
            {
                case nameof(UserCreatedIntegrationEvent):
                    _logger.LogDebug("Documents service handling integration event: {NotificationName} {@Notification}", nameof(UserCreatedIntegrationEvent), message);
                    var userCreatedEvent = JsonSerializer.Deserialize<UserCreatedIntegrationEvent>(message.Message.Value);
                    HandleUserCreatedEvent(userCreatedEvent!);
                    _logger.LogInformation("Documents service finished integration event: {NotificationName} {@Response}", nameof(UserCreatedIntegrationEvent), message);
                    break;

                case nameof(UserUpdatedIntegrationEvent):
                    _logger.LogDebug("Documents service handling integration event: {NotificationName} {@Notification}", nameof(UserUpdatedIntegrationEvent), message);
                    var userUpdatedEvent = JsonSerializer.Deserialize<UserUpdatedIntegrationEvent>(message.Message.Value);
                    HandleUserUpdatedEvent(userUpdatedEvent!);
                    _logger.LogInformation("Documents service finished integration event: {NotificationName} {@Response}", nameof(UserUpdatedIntegrationEvent), message);
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
        var header = headers.FirstOrDefault(h => h.Key == "eventType");
        return header != null ? Encoding.UTF8.GetString(header.GetValueBytes()) : null;
    }

    private void HandleUserCreatedEvent(UserCreatedIntegrationEvent userCreatedEvent)
    {
        _mediator.Publish(new UserAddedEvent
        {
            User = new User
            {
                Name = userCreatedEvent.Name,
                Id = userCreatedEvent.UserId,
                Role = userCreatedEvent.Role
            }
        });
    }

    private void HandleUserUpdatedEvent(UserUpdatedIntegrationEvent userUpdatedEvent)
    {
        _mediator.Publish(new UserUpdatedEvent
        {
            UserId = userUpdatedEvent.UserId,
            UserName = userUpdatedEvent.Name
        });
    }

    public override void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
        base.Dispose();
    }
}
