using Common.DomainEvents;
using Documents.Application.Interfaces;
using MediatR;

namespace Documents.Application.Commands;

public record AbandonProcessCommand(Guid UserId, Guid CustomerId) : IRequest<Unit>;

public class AbandonProcessCommandHandler(IDomainEventDispatcher dispatcher, IDocumentInventoryRepository repository) 
    : IRequestHandler<AbandonProcessCommand, Unit>
{
    public async Task<Unit> Handle(AbandonProcessCommand command, CancellationToken cancellationToken)
    {
        var documentInventory = await repository.GetDocumentInventory(cancellationToken);
        documentInventory.AbandonProcess(command.UserId, command.CustomerId);
        await dispatcher.DispatchEvents(documentInventory.BusinessEvents, cancellationToken);

        return Unit.Value;
    }
}