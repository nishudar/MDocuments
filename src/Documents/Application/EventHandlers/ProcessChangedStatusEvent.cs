using Common.DomainEvents;
using Common.IntegrationEvents;
using Common.IntegrationEvents.Events;
using Documents.Application.Interfaces;
using Documents.Domain.Events;

namespace Documents.Application.EventHandlers;

public class ProcessChangedStatusEventHandler(
    IDocumentInventoryRepository repository,
    IIntegrationEventProducer integrationEventProducer)
    : IDomainEventHandler<ProcessChangedStatusEvent>
{
    public async Task Handle(ProcessChangedStatusEvent domainEvent, CancellationToken cancellationToken)
    {
        var process = domainEvent.Process;
        await repository.UpdateProcessStatus(process, cancellationToken);
        
        var @event = new ProcessStatusUpdate(process.Id, process.BusinessUserId, process.CustomerId, process.Status);
        await integrationEventProducer.SendEvent(IntegrationTopics.DocumentsTopic, "process",@event, cancellationToken);
    }
}