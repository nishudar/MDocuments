using Common.Abstracts;
using Force.DeepCloner;
using Users.Domain.DomainEvents;
using Users.Domain.Entities;
using Users.Domain.Exceptions;

namespace Users.Domain.Aggregates;

internal class UsersInventory(ICollection<User> users)
    : Aggregate, IUsersInventory
{
    private List<User> Users { get; } = users.ToList();
    
    public Task<User> AddUser(string userName, string role)
    {
        var user = new User
        {
            Name = userName,
            Role = role,
            Id = Guid.NewGuid()
        };
        PublishBusinessEvent(new UserAddedEvent {User = user});

        return Task.FromResult(user.DeepClone());
    }

    public Task<User> UpdateUser(Guid guid, string name)
    {
        var existingUser = Users.Find(u => u.Id == guid);
        if (existingUser is null)
            throw new UserDoesNotExistException(guid);
        
        existingUser.Name = name;
        PublishBusinessEvent(new UserUpdatedEvent
        {
            User = existingUser
        });
        return Task.FromResult(existingUser.DeepClone());
    }

    public Task<IEnumerable<User>> GetUsers()
    {
        return Task.FromResult(Users.DeepClone().AsEnumerable());
    }
}