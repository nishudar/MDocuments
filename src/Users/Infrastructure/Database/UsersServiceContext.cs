using Microsoft.EntityFrameworkCore;
using Users.Domain.Entities;

namespace Users.Infrastructure.Database;

internal class UsersServiceContext(DbContextOptions<UsersServiceContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; init; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseInMemoryDatabase("UsersServiceDatabase")
            .EnableSensitiveDataLogging();
    }
}
    