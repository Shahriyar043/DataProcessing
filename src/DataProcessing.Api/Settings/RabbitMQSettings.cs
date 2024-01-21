namespace DataProcessing.Api.Settings;

public class RabbitMQSettings
{
    public string? HostName { get; set; }
    public string? QueueName { get; set; }
    public int BatchSize { get; set; }
}
