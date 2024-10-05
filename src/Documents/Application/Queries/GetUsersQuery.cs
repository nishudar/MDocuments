using Documents.Application.Interfaces;
using Documents.Domain.Entities;

namespace Documents.Application.Queries;

using MediatR;

public record GetBusinessUsersQuery : IRequest<IEnumerable<BusinessUser>> { }

public class GetBusinessUsersHandler(IDocumentInventoryRepository repository) : IRequestHandler<GetBusinessUsersQuery, IEnumerable<BusinessUser>>
{
    public async Task<IEnumerable<BusinessUser>> Handle(GetBusinessUsersQuery request, CancellationToken cancellationToken)
    {
        var documentInventory = await repository.GetDocumentInventory(cancellationToken);
        var users = documentInventory.GetUsers();
        
        return users;
    }
}