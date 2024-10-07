namespace NotificationHub.Notifications;

internal static class Extension
{
    public static void AddNotificatioNHub(this IServiceCollection services)
    {
        services.AddSignalR();
    }

    public static void UseNotificationHub(this WebApplication app)
    {
        app.MapHub<NotificationHub>("/notificationHub");
    }
}