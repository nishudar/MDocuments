namespace NotificationHub.Consumer;

internal class KafkaConsumerConfiguration
{
    public string? Group { get; set; }
    public string? Server { get; set; }
}