using Microsoft.EntityFrameworkCore;
using Users.Application.Interfaces;
using Users.Domain.Aggregates;
using Users.Infrastructure.Database;

namespace Users.Infrastructure;

internal static class Extension
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<UsersServiceContext>(options =>
        {
            options.UseInMemoryDatabase("UsersServiceDatabase")
                .EnableSensitiveDataLogging();
        });
        services.AddSingleton<IUsersUnitOfWork, UsersUnitOfWork>();
    }
    
    public static void UseInfrastructure(this WebApplication webApplication)
    {
        using var scope = webApplication.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<UsersServiceContext>();
        dbContext.Database.EnsureCreated();
    }
    
    
    public static async Task<IUsersAggregateRoot> GetUsersInventory(this IUsersUnitOfWork unitOfWork, CancellationToken ct = default)
    {
        var users = await unitOfWork.GetUsers(ct);

        return new UsersAggregateRoot(users);
    }
}