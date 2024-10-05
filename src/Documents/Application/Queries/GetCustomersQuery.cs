using Documents.Application.Interfaces;
using Documents.Domain.Entities;
namespace Documents.Application.Queries;

using MediatR;

public record GetCustomersQuery : IRequest<IEnumerable<Customer>> { }

public class GetCustomersHandler(IDocumentInventoryRepository repository) : IRequestHandler<GetCustomersQuery, IEnumerable<Customer>>
{
    public async Task<IEnumerable<Customer>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var documentInventory = await repository.GetDocumentInventory(cancellationToken);
        var customers = documentInventory.GetCustomers();
        
        return customers;
    }
}