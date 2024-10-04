using Common.DomainEvents;
using Documents.Application.Interfaces;
using Documents.Domain.Events;

namespace Documents.Application.EventHandlers;

public class DocumentAddedEventHandler(IDocumentInventoryRepository repository) : IDomainEventHandler<DocumentAddedEvent>
{
    public async Task Handle(DocumentAddedEvent domainEvent, CancellationToken cancellationToken)
    {
        await repository.AddDocument(domainEvent.Document, cancellationToken);
    }
}