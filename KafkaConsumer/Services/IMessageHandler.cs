using Confluent.Kafka;

namespace KafkaConsumer.Services;

public interface IMessageHandler
{
    Task HandleMessageAsync(ConsumeResult<string, string> consumeResult, CancellationToken cancellationToken = default);
}
