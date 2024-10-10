using Common.DomainEvents;
using Documents.Application.Interfaces;
using Documents.Infrastructure;
using MediatR;

namespace Documents.Application.Commands;

internal record AbandonProcessCommand(Guid UserId, Guid CustomerId) : IRequest<Unit>;

internal class AbandonProcessCommandHandler(IDomainEventDispatcher dispatcher, IDocumentsUnitOfWork unitOfWork)
    : IRequestHandler<AbandonProcessCommand, Unit>
{
    public async Task<Unit> Handle(AbandonProcessCommand command, CancellationToken cancellationToken)
    {
        //Here i'd call user & consumer domain services to verify the users
        var documentInventory = await unitOfWork.GetDocumentInventory(cancellationToken);
        documentInventory.AbandonProcess(command.UserId, command.CustomerId);
        await dispatcher.DispatchEvents(documentInventory.BusinessEvents, cancellationToken);

        return Unit.Value;
    }
}