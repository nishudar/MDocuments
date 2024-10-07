using Common.DomainEvents;
using Common.IntegrationEvents.Events;
using Documents.Application.Interfaces;
using Documents.Domain.Events;
using MediatR;

namespace Documents.Application.EventHandlers;

internal class DocumentAddedEventHandler(IDocumentInventoryRepository repository, IMediator mediator)
    : IDomainEventHandler<DocumentAddedEvent>
{
    public async Task Handle(DocumentAddedEvent domainEvent, CancellationToken cancellationToken)
    {
        await repository.AddDocument(domainEvent.Document, cancellationToken);
        await mediator.Publish(
            new DocumentAddedIntegrationEvent(domainEvent.Process.Id, domainEvent.Process.BusinessUserId,
                domainEvent.Document.CustomerId, domainEvent.Document.Id), cancellationToken);
    }
}