using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using KafkaConsumer.Services;

namespace KafkaConsumer.Handlers;

public class MessageHandler : IMessageHandler
{
    private readonly ILogger<MessageHandler> _logger;

    public MessageHandler(ILogger<MessageHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleMessageAsync(ConsumeResult<string, string> consumeResult, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Message Received - Key: {Key}, Value: {Value}, Partition: {Partition}, Offset: {Offset}, Timestamp: {Timestamp}",
            consumeResult.Message.Key,
            consumeResult.Message.Value,
            consumeResult.Partition.Value,
            consumeResult.Offset.Value,
            consumeResult.Message.Timestamp.UtcDateTime);

        // Add your business logic here

        return Task.CompletedTask;
    }
}
