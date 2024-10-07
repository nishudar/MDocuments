using Common.DomainEvents;
using Documents.Application.Interfaces;
using MediatR;

namespace Documents.Application.Commands;

internal record AbandonProcessCommand(Guid UserId, Guid CustomerId) : IRequest<Unit>;

internal class AbandonProcessCommandHandler(IDomainEventDispatcher dispatcher, IDocumentInventoryRepository repository)
    : IRequestHandler<AbandonProcessCommand, Unit>
{
    public async Task<Unit> Handle(AbandonProcessCommand command, CancellationToken cancellationToken)
    {
        //Here i'd call user & consumer domain services to verify the users

        var documentInventory = await repository.GetDocumentInventory(cancellationToken);
        documentInventory.AbandonProcess(command.UserId, command.CustomerId);
        await dispatcher.DispatchEvents(documentInventory.BusinessEvents, cancellationToken);

        return Unit.Value;
    }
}