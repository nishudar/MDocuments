using Common.Abstracts;
using Users.Domain.Entities;

namespace Users.Domain.Aggregates;

internal interface IUsersAggregateRoot : IAggregateRoot
{
    Task<User> AddUser(string userName, string role);
    Task<User> UpdateUser(Guid guid, string name);
    Task<IEnumerable<User>> GetUsers();
}