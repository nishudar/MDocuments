using Common.DomainEvents;
using Documents.Application.Interfaces;
using Documents.Infrastructure;
using MediatR;

namespace Documents.Application.Commands;

internal record FinishProcessCommand(Guid UserId, Guid CustomerId) : IRequest<Unit>;

internal class FinishProcessCommandHandler(IDomainEventDispatcher dispatcher, IDocumentsUnitOfWork unitOfWork)
    : IRequestHandler<FinishProcessCommand, Unit>
{
    public async Task<Unit> Handle(FinishProcessCommand command, CancellationToken cancellationToken)
    {
        //Here i'd call user & consumer domain services to verify the users

        var documentInventory = await unitOfWork.GetDocumentInventory(cancellationToken);
        documentInventory.FinishProcess(command.UserId, command.CustomerId);
        await dispatcher.DispatchEvents(documentInventory.BusinessEvents, cancellationToken);

        return Unit.Value;
    }
}