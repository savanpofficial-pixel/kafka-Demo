namespace KafkaConsumer.Configuration;

public class KafkaConsumerSettings
{
    public const string SectionName = "KafkaConsumer";

    public string BootstrapServers { get; set; } = string.Empty;
    public string GroupId { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string AutoOffsetReset { get; set; } = "Earliest";
    public bool EnableAutoCommit { get; set; } = true;
}
