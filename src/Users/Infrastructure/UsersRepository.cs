using System.Collections.Concurrent;
using Force.DeepCloner;
using Users.Application.Interfaces;
using Users.Domain.Aggregates;
using Users.Domain.Entities;
using Users.Domain.Exceptions;

namespace Users.Infrastructure;

internal class UsersRepository : IUsersRepository
{
    private ConcurrentBag<BusinessUser> Users { get; } = [];
    
    public Task<IUsersInventory> GetUsersInventory(CancellationToken cancellationToken)
    {
        return Task.FromResult<IUsersInventory>(new UsersInventory(
            Users.ToArray().DeepClone()));
    }
    
    public Task<Guid> AddBusinessUser(BusinessUser user, CancellationToken ct)
    {
        Users.Add(new BusinessUser
        {
            Name = user.Name,
            Id = user.Id,
            Role = user.Role
        });

        return Task.FromResult(user.Id);
    }

    public Task UpdateBusinessUser(BusinessUser user, CancellationToken ct)
    {
        var existingUser = Users.FirstOrDefault(u => u.Id == user.Id);
        if (existingUser is null)
            throw new UserDoesNotExistException();

        existingUser.Name = user.Name;
        
        return Task.CompletedTask;
    }
}