using Documents.Application.Interfaces;
using Documents.Domain.Entities;
using MediatR;

namespace Documents.Application.Queries;

public record GetCustomersQuery : IRequest<IEnumerable<Customer>>
{
}

public class GetCustomersHandler(IDocumentInventoryRepository repository)
    : IRequestHandler<GetCustomersQuery, IEnumerable<Customer>>
{
    public async Task<IEnumerable<Customer>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var documentInventory = await repository.GetDocumentInventory(cancellationToken);
        var customers = documentInventory.GetCustomers();

        return customers;
    }
}