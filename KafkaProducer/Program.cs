using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using KafkaProducer.Configuration;
using KafkaProducer.Services;

namespace KafkaProducer;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.Configure<KafkaProducerSettings>(
                    context.Configuration.GetSection(KafkaProducerSettings.SectionName));

                services.AddSingleton<IKafkaProducerService, KafkaProducerService>();
            })
            .Build();

        var logger = host.Services.GetRequiredService<ILogger<Program>>();
        var producerService = host.Services.GetRequiredService<IKafkaProducerService>();

        logger.LogInformation("Kafka Producer started. Press Ctrl+C to exit.");

        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
            logger.LogInformation("Shutdown requested...");
        };

        try
        {
            var success = await producerService.ProduceMessageAsync(
                "message-key",
                "Hello from .NET Kafka Producer",
                cts.Token);

            if (success)
            {
                logger.LogInformation("Message sent successfully!");
            }
            else
            {
                logger.LogWarning("Message failed to send");
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Operation cancelled");
        }

        await host.StopAsync();
    }
}
