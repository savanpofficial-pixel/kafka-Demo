using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using KafkaProducer.Configuration;

namespace KafkaProducer.Services;

public class KafkaProducerService : IKafkaProducerService, IDisposable
{
    private readonly ILogger<KafkaProducerService> _logger;
    private readonly KafkaProducerSettings _settings;
    private readonly IProducer<string, string> _producer;

    public KafkaProducerService(
        ILogger<KafkaProducerService> logger,
        IOptions<KafkaProducerSettings> settings)
    {
        _logger = logger;
        _settings = settings.Value;

        var config = new ProducerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            ClientId = _settings.ClientId
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
        _logger.LogInformation("Kafka Producer initialized with bootstrap servers: {BootstrapServers}",
            _settings.BootstrapServers);
    }

    public async Task<bool> ProduceMessageAsync(string key, string value, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Sending message with key: {Key}", key);

            var message = new Message<string, string>
            {
                Key = key,
                Value = value
            };

            var deliveryResult = await _producer.ProduceAsync(_settings.Topic, message, cancellationToken);

            _logger.LogInformation(
                "Message delivered to {TopicPartitionOffset}. Status: {Status}, Partition: {Partition}, Offset: {Offset}",
                deliveryResult.TopicPartitionOffset,
                deliveryResult.Status,
                deliveryResult.Partition.Value,
                deliveryResult.Offset.Value);

            return deliveryResult.Status == PersistenceStatus.Persisted;
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex, "Failed to deliver message: {Reason}", ex.Error.Reason);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while producing message");
            return false;
        }
    }

    public void Dispose()
    {
        _producer?.Dispose();
        _logger.LogInformation("Kafka Producer disposed");
    }
}
