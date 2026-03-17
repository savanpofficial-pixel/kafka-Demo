using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using KafkaConsumer.Configuration;

namespace KafkaConsumer.Services;

public class KafkaConsumerService : IKafkaConsumerService, IDisposable
{
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly KafkaConsumerSettings _settings;
    private readonly IMessageHandler _messageHandler;
    private readonly IConsumer<string, string> _consumer;

    public KafkaConsumerService(
        ILogger<KafkaConsumerService> logger,
        IOptions<KafkaConsumerSettings> settings,
        IMessageHandler messageHandler)
    {
        _logger = logger;
        _settings = settings.Value;
        _messageHandler = messageHandler;

        var config = new ConsumerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            GroupId = _settings.GroupId,
            AutoOffsetReset = Enum.Parse<AutoOffsetReset>(_settings.AutoOffsetReset, true),
            EnableAutoCommit = _settings.EnableAutoCommit
        };

        _consumer = new ConsumerBuilder<string, string>(config).Build();
        _consumer.Subscribe(_settings.Topic);

        _logger.LogInformation(
            "Kafka Consumer initialized. Subscribed to topic: {Topic}, GroupId: {GroupId}",
            _settings.Topic,
            _settings.GroupId);
    }

    public async Task StartConsumingAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Kafka Consumer started. Listening to '{Topic}' topic...", _settings.Topic);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(TimeSpan.FromMilliseconds(1000));

                    if (consumeResult != null)
                    {
                        await _messageHandler.HandleMessageAsync(consumeResult, cancellationToken);
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming message: {Reason}", ex.Error.Reason);
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Consumer cancelled");
        }
        finally
        {
            _consumer.Close();
            _logger.LogInformation("Consumer closed");
        }
    }

    public void Dispose()
    {
        _consumer?.Dispose();
        _logger.LogInformation("Kafka Consumer disposed");
    }
}
