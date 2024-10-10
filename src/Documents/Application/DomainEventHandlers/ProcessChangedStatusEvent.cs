using Common.DomainEvents;
using Common.IntegrationEvents.Events;
using Documents.Application.Interfaces;
using Documents.Domain.Events;
using MediatR;

namespace Documents.Application.DomainEventHandlers;

internal class ProcessChangedStatusEventHandler(
    IDocumentsUnitOfWork unitOfWork,
    IMediator mediator)
    : IDomainEventHandler<ProcessChangedStatusEvent>
{
    public async Task Handle(ProcessChangedStatusEvent domainEvent, CancellationToken cancellationToken)
    {
        var process = domainEvent.Process;
        await unitOfWork.UpdateProcessStatus(process, cancellationToken);
        await unitOfWork.SaveChanges(cancellationToken);

        await mediator.Publish(
            new ProcessStatusUpdateIntegrationEvent(process.Id,
                process.OperatorUserId,
                process.CustomerId,
                process.Status), 
            cancellationToken);
    }
}