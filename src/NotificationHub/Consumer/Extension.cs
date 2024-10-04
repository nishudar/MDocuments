namespace NotificationHub.Consumer;

public static class Extension
{
    public static void AddConsumerBackgroundService(this IServiceCollection services, ConfigurationManager configurationManager)
    {
        services.Configure<KafkaConsumerConfiguration>(configurationManager.GetSection("Kafka"));
        services.AddHostedService<KafkaConsumerService>();
    }
}