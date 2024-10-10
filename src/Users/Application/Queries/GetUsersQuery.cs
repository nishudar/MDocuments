using MediatR;
using Users.Application.Interfaces;
using Users.Domain.Entities;
using Users.Infrastructure;

namespace Users.Application.Queries;

internal record GetUsersQuery : IRequest<IEnumerable<User>> { }

internal class GetUsersHandler(IUsersUnitOfWork unitOfWork)
    : IRequestHandler<GetUsersQuery, IEnumerable<User>>
{
    public async Task<IEnumerable<User>> Handle(GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        var documentInventory = await unitOfWork.GetUsersInventory(cancellationToken);
        var users = await documentInventory.GetUsers();

        return users;
    }
}