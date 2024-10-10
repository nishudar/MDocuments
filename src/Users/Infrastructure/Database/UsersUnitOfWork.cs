using Common.Database;
using Microsoft.EntityFrameworkCore;
using Users.Application.Interfaces;
using Users.Domain.Entities;
using Users.Domain.Exceptions;

namespace Users.Infrastructure.Database;


internal class UsersUnitOfWork(UsersServiceContext ctx) : EfUnitOfWork<UsersServiceContext>(ctx), IUsersUnitOfWork
{
    private readonly UsersServiceContext _ctx = ctx;

    public async Task<ICollection<User>> GetUsers(CancellationToken ct)
        => await _ctx.Users.AsNoTracking().ToArrayAsync(ct);
    
    
    public Task<Guid> AddUser(User user, CancellationToken ct)
    {
        _ctx.Users.Add(new User
        {
            Name = user.Name,
            Id = user.Id,
            Role = user.Role
        });

        return Task.FromResult(user.Id);
    }

    public Task UpdateUser(User user, CancellationToken ct)
    {
        var existingUser = _ctx.Users.FirstOrDefault(u => u.Id == user.Id);
        if (existingUser is null)
            throw new UserDoesNotExistException();

        existingUser.Name = user.Name;
        
        return Task.CompletedTask;
    }
}