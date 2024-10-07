using Common.Abstracts;
using Users.Domain.Entities;

namespace Users.Domain.Aggregates;

public interface IUsersInventory : IAggregate
{
    Task<BusinessUser> AddBusinessUser(string name);
    Task<BusinessUser> UpdateBusinessUser(Guid guid, BusinessUser businessUser);
    Task<IEnumerable<BusinessUser>> GetUsers();
}