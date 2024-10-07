using Users.Application.Interfaces;

namespace Users.Infrastructure;

public static class Extension
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IUsersRepository, UsersRepository>();
    }
}