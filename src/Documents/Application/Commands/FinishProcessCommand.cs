using Common.DomainEvents;
using Documents.Application.Interfaces;
using MediatR;

namespace Documents.Application.Commands;

public record FinishProcessCommand(Guid UserId, Guid CustomerId) : IRequest<Unit>;

public class FinishProcessCommandHandler(IDomainEventDispatcher dispatcher, IDocumentInventoryRepository repository) 
    : IRequestHandler<FinishProcessCommand, Unit>
{
    public async Task<Unit> Handle(FinishProcessCommand command, CancellationToken cancellationToken)
    {
        var documentInventory = await repository.GetDocumentInventory(cancellationToken);
        documentInventory.FinishProcess(command.UserId, command.CustomerId);
        await dispatcher.DispatchEvents(documentInventory.BusinessEvents, cancellationToken);

        return Unit.Value;
    }
}