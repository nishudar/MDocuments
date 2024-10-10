using Documents.Application.Interfaces;
using Documents.Domain.Entities;
using Documents.Infrastructure;
using MediatR;

namespace Documents.Application.Queries;

internal record GetUsersQuery : IRequest<IEnumerable<User>>
{
}

internal class GetUsersHandler(IDocumentsUnitOfWork unitOfWork)
    : IRequestHandler<GetUsersQuery, IEnumerable<User>>
{
    public async Task<IEnumerable<User>> Handle(GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        var documentInventory = await unitOfWork.GetDocumentInventory(cancellationToken);
        var users = documentInventory.GetUsers();

        return users;
    }
}