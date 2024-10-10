using Common.DomainEvents;
using Documents.Application.Interfaces;
using Documents.Domain.ValueObjects;
using Documents.Infrastructure;
using MediatR;

namespace Documents.Application.Commands;

internal record StartProcessCommand(Guid UserId, Guid CustomerId) : IRequest<ProcessReport>;

internal class StartProcessCommandHandler(IDomainEventDispatcher dispatcher, IDocumentsUnitOfWork unitOfWork)
    : IRequestHandler<StartProcessCommand, ProcessReport>
{
    public async Task<ProcessReport> Handle(StartProcessCommand command, CancellationToken cancellationToken)
    {
        //Here i'd call user & consumer domain services to verify the users

        var documentInventory = await unitOfWork.GetDocumentInventory(cancellationToken);
        var process = documentInventory.StartProcess(command.UserId, command.CustomerId);
        await dispatcher.DispatchEvents(documentInventory.BusinessEvents, cancellationToken);
        var report = documentInventory.GetReport(process.Id)!;

        return report;
    }
}