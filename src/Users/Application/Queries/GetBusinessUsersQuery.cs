using MediatR;
using Users.Application.Interfaces;
using Users.Domain.Entities;

namespace Users.Application.Queries;

internal record GetBusinessUsersQuery : IRequest<IEnumerable<BusinessUser>> { }

internal class GetBusinessUsersHandler(IUsersRepository repository)
    : IRequestHandler<GetBusinessUsersQuery, IEnumerable<BusinessUser>>
{
    public async Task<IEnumerable<BusinessUser>> Handle(GetBusinessUsersQuery request,
        CancellationToken cancellationToken)
    {
        var documentInventory = await repository.GetUsersInventory(cancellationToken);
        var users = await documentInventory.GetUsers();

        return users;
    }
}