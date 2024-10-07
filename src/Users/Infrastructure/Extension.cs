using Users.Application.Interfaces;

namespace Users.Infrastructure;

internal static class Extension
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IUsersRepository, UsersRepository>();
    }
}