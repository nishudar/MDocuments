using Users.Domain.Aggregates;
using Users.Domain.Entities;

namespace Users.Application.Interfaces;

public interface IUsersRepository
{
    Task<IUsersInventory> GetUsersInventory(CancellationToken cancellationToken);
    Task<Guid> AddBusinessUser(BusinessUser user, CancellationToken ct);
    Task UpdateBusinessUser(BusinessUser user, CancellationToken ct);
}