namespace KafkaProducer.Configuration;

public class KafkaProducerSettings
{
    public const string SectionName = "KafkaProducer";

    public string BootstrapServers { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
}
