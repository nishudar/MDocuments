using Common.DomainEvents;
using Common.IntegrationEvents.Events;
using Documents.Application.Interfaces;
using Documents.Domain.Events;
using MediatR;

namespace Documents.Application.EventHandlers;

public class ProcessChangedStatusEventHandler(
    IDocumentInventoryRepository repository,
    IMediator mediator)
    : IDomainEventHandler<ProcessChangedStatusEvent>
{
    public async Task Handle(ProcessChangedStatusEvent domainEvent, CancellationToken cancellationToken)
    {
        var process = domainEvent.Process;
        await repository.UpdateProcessStatus(process, cancellationToken);
        
        await mediator.Publish(new ProcessStatusUpdateIntegrationEvent(process.Id, process.BusinessUserId, process.CustomerId, process.Status), cancellationToken);
    }   
}