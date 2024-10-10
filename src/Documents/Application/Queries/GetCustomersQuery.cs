using Documents.Application.Interfaces;
using Documents.Domain.Entities;
using Documents.Domain.Enums;
using Documents.Infrastructure;
using MediatR;

namespace Documents.Application.Queries;

internal record GetCustomersQuery : IRequest<IEnumerable<User>>
{
}

internal class GetCustomersHandler(IDocumentsUnitOfWork unitOfWork)
    : IRequestHandler<GetCustomersQuery, IEnumerable<User>>
{
    public async Task<IEnumerable<User>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var documentInventory = await unitOfWork.GetDocumentInventory(cancellationToken);
        var customers = documentInventory.GetUsers().Where(customer => customer.Role == UserRole.Customer);

        return customers;
    }
}