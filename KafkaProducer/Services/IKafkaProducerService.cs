namespace KafkaProducer.Services;

public interface IKafkaProducerService
{
    Task<bool> ProduceMessageAsync(string key, string value, CancellationToken cancellationToken = default);
}
