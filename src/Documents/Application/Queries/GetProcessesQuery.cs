using Documents.Application.Interfaces;
using Documents.Domain.Entities;
namespace Documents.Application.Queries;

using MediatR;

public record GetProcessesQuery : IRequest<IEnumerable<Process>> { }

public class GetProcessesHandler(IDocumentInventoryRepository repository) : IRequestHandler<GetProcessesQuery, IEnumerable<Process>>
{
    public async Task<IEnumerable<Process>> Handle(GetProcessesQuery request, CancellationToken cancellationToken)
    {
        var documentInventory = await repository.GetDocumentInventory(cancellationToken);
        var processes = documentInventory.GetProcesses();
        
        return processes;
    }
}