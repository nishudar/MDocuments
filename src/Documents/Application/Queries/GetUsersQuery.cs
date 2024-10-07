using Documents.Application.Interfaces;
using Documents.Domain.Entities;
using MediatR;

namespace Documents.Application.Queries;

internal record GetBusinessUsersQuery : IRequest<IEnumerable<User>>
{
}

internal class GetBusinessUsersHandler(IDocumentInventoryRepository repository)
    : IRequestHandler<GetBusinessUsersQuery, IEnumerable<User>>
{
    public async Task<IEnumerable<User>> Handle(GetBusinessUsersQuery request,
        CancellationToken cancellationToken)
    {
        var documentInventory = await repository.GetDocumentInventory(cancellationToken);
        var users = documentInventory.GetUsers();

        return users;
    }
}