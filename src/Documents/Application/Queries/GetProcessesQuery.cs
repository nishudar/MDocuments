using Documents.Application.Interfaces;
using Documents.Domain.Entities;
using Documents.Infrastructure;
using MediatR;

namespace Documents.Application.Queries;

internal record GetProcessesQuery : IRequest<IEnumerable<Process>>
{
}

internal class GetProcessesHandler(IDocumentsUnitOfWork unitOfWork)
    : IRequestHandler<GetProcessesQuery, IEnumerable<Process>>
{
    public async Task<IEnumerable<Process>> Handle(GetProcessesQuery request, CancellationToken cancellationToken)
    {
        var documentInventory = await unitOfWork.GetDocumentInventory(cancellationToken);
        var processes = documentInventory.GetProcesses();

        return processes;
    }
}