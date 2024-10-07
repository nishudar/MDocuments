using Documents.Application.Interfaces;
using Documents.Domain.Entities;
using MediatR;

namespace Documents.Application.Queries;

internal record GetCustomersQuery : IRequest<IEnumerable<User>>
{
}

internal class GetCustomersHandler(IDocumentInventoryRepository repository)
    : IRequestHandler<GetCustomersQuery, IEnumerable<User>>
{
    public async Task<IEnumerable<User>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var documentInventory = await repository.GetDocumentInventory(cancellationToken);
        var customers = documentInventory.GetUsers();

        return customers;
    }
}