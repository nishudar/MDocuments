using Common.DomainEvents;
using Common.IntegrationEvents.Events;
using Documents.Application.Interfaces;
using Documents.Domain.Events;
using MediatR;

namespace Documents.Application.DomainEventHandlers;

internal class DocumentAddedEventHandler(IDocumentsUnitOfWork unitOfWork, IMediator mediator)
    : IDomainEventHandler<DocumentAddedEvent>
{
    public async Task Handle(DocumentAddedEvent domainEvent, CancellationToken cancellationToken)
    {
        await unitOfWork.AddDocument(domainEvent.Document, cancellationToken);
        await unitOfWork.SaveChanges(cancellationToken);
        await mediator.Publish(
            new DocumentAddedIntegrationEvent(domainEvent.Process.Id, domainEvent.Process.OperatorUserId,
                domainEvent.Document.CustomerId, domainEvent.Document.Id), cancellationToken);
    }
}