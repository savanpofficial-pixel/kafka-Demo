namespace KafkaConsumer.Services;

public interface IKafkaConsumerService
{
    Task StartConsumingAsync(CancellationToken cancellationToken = default);
}
