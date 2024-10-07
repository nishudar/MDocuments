using Documents.Application.Interfaces;
using Documents.Domain.Entities;
using MediatR;

namespace Documents.Application.Queries;

internal record GetProcessesQuery : IRequest<IEnumerable<Process>>
{
}

internal class GetProcessesHandler(IDocumentInventoryRepository repository)
    : IRequestHandler<GetProcessesQuery, IEnumerable<Process>>
{
    public async Task<IEnumerable<Process>> Handle(GetProcessesQuery request, CancellationToken cancellationToken)
    {
        var documentInventory = await repository.GetDocumentInventory(cancellationToken);
        var processes = documentInventory.GetProcesses();

        return processes;
    }
}