using Common.Abstracts;
using Force.DeepCloner;
using Users.Domain.DomainEvents;
using Users.Domain.Entities;

namespace Users.Domain.Aggregates;

public class UsersInventory(ICollection<BusinessUser> users)
    : Aggregate, IUsersInventory
{
    private List<BusinessUser> Users { get; } = users.ToList();
    
    public Task<BusinessUser> AddBusinessUser(string name)
    {
        var businessUser = new BusinessUser
        {
            Name = name,
            Id = Guid.NewGuid()
        };
        AddBusinessEvent(new BusinessUserAddedEvent {User = businessUser});

        return Task.FromResult(businessUser);
    }

    public Task<BusinessUser> UpdateBusinessUser(Guid guid, BusinessUser businessUser)
    {
        var existingUser = Users.Find(u => u.Id == businessUser.Id);
        existingUser.Name = businessUser.Name;

        AddBusinessEvent(new BusinessUserUpdatedEvent
        {
            User = businessUser
        });

        return Task.FromResult(businessUser);
    }

    public Task<IEnumerable<BusinessUser>> GetUsers()
    {
        return Task.FromResult(Users.DeepClone().AsEnumerable());
    }
}