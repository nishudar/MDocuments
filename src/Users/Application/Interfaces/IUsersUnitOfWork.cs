using Common.Database;
using Users.Domain.Entities;

namespace Users.Application.Interfaces;

internal interface IUsersUnitOfWork : IUnitOfWork
{
    Task<Guid> AddUser(User user, CancellationToken ct);
    Task UpdateUser(User user, CancellationToken ct);
    Task<ICollection<User>> GetUsers(CancellationToken ct);
}
