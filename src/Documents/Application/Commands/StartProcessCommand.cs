using Common.DomainEvents;
using Documents.Application.Interfaces;
using Documents.Domain.ValueObjects;
using MediatR;

namespace Documents.Application.Commands;

public record StartProcessCommand(Guid UserId, Guid CustomerId) : IRequest<ProcessReport>;

public class StartProcessCommandHandler(IDomainEventDispatcher dispatcher, IDocumentInventoryRepository repository) 
    : IRequestHandler<StartProcessCommand, ProcessReport>
{
    public async Task<ProcessReport> Handle(StartProcessCommand command, CancellationToken cancellationToken)
    {
        //Here i'd call user & consumer domain services to verify the users
        
        var documentInventory = await repository.GetDocumentInventory(cancellationToken);
        var process = documentInventory.StartProcess(command.UserId, command.CustomerId);
        await dispatcher.DispatchEvents(documentInventory.BusinessEvents, cancellationToken);
        var report = documentInventory.GetReport(process.Id)!;
            
        return report;
    }
}