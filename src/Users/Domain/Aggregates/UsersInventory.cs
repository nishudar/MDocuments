using Common.Abstracts;
using Force.DeepCloner;
using Users.Domain.DomainEvents;
using Users.Domain.Entities;
using Users.Domain.Exceptions;

namespace Users.Domain.Aggregates;

internal class UsersInventory(ICollection<BusinessUser> users)
    : Aggregate, IUsersInventory
{
    private List<BusinessUser> Users { get; } = users.ToList();
    
    public Task<BusinessUser> AddBusinessUser(string userName, string role)
    {
        var businessUser = new BusinessUser
        {
            Name = userName,
            Role = role,
            Id = Guid.NewGuid()
        };
        AddBusinessEvent(new BusinessUserAddedEvent {User = businessUser});

        return Task.FromResult(businessUser.DeepClone());
    }

    public Task<BusinessUser> UpdateBusinessUser(Guid guid, string name)
    {
        var existingUser = Users.Find(u => u.Id == guid);
        if (existingUser is null)
            throw new UserDoesNotExistException(guid);
        
        existingUser.Name = name;
        AddBusinessEvent(new BusinessUserUpdatedEvent
        {
            User = existingUser
        });
        return Task.FromResult(existingUser.DeepClone());
    }

    public Task<IEnumerable<BusinessUser>> GetUsers()
    {
        return Task.FromResult(Users.DeepClone().AsEnumerable());
    }
}