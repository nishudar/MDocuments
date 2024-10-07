using Documents.Application.Interfaces;
using Documents.Domain.Entities;
using MediatR;

namespace Documents.Application.Queries;

internal record GetCustomersQuery : IRequest<IEnumerable<Customer>>
{
}

internal class GetCustomersHandler(IDocumentInventoryRepository repository)
    : IRequestHandler<GetCustomersQuery, IEnumerable<Customer>>
{
    public async Task<IEnumerable<Customer>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var documentInventory = await repository.GetDocumentInventory(cancellationToken);
        var customers = documentInventory.GetCustomers();

        return customers;
    }
}