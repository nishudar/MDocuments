using Common.Abstracts;
using Users.Domain.Entities;

namespace Users.Domain.Aggregates;

internal interface IUsersInventory : IAggregate
{
    Task<BusinessUser> AddBusinessUser(string userName, string role);
    Task<BusinessUser> UpdateBusinessUser(Guid guid, string name);
    Task<IEnumerable<BusinessUser>> GetUsers();
}