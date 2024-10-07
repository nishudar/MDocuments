using Documents.Application.Interfaces;
using Documents.Domain.Entities;
using MediatR;

namespace Documents.Application.Queries;

internal record GetBusinessUsersQuery : IRequest<IEnumerable<BusinessUser>>
{
}

internal class GetBusinessUsersHandler(IDocumentInventoryRepository repository)
    : IRequestHandler<GetBusinessUsersQuery, IEnumerable<BusinessUser>>
{
    public async Task<IEnumerable<BusinessUser>> Handle(GetBusinessUsersQuery request,
        CancellationToken cancellationToken)
    {
        var documentInventory = await repository.GetDocumentInventory(cancellationToken);
        var users = documentInventory.GetUsers();

        return users;
    }
}