using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using KafkaConsumer.Configuration;
using KafkaConsumer.Services;
using KafkaConsumer.Handlers;

namespace KafkaConsumer;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.Configure<KafkaConsumerSettings>(
                    context.Configuration.GetSection(KafkaConsumerSettings.SectionName));

                services.AddSingleton<IMessageHandler, MessageHandler>();
                services.AddSingleton<IKafkaConsumerService, KafkaConsumerService>();
            })
            .Build();

        var logger = host.Services.GetRequiredService<ILogger<Program>>();
        var consumerService = host.Services.GetRequiredService<IKafkaConsumerService>();

        logger.LogInformation("Starting Kafka Consumer. Press Ctrl+C to exit.");

        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
            logger.LogInformation("Shutdown requested...");
        };

        try
        {
            await consumerService.StartConsumingAsync(cts.Token);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred");
        }

        await host.StopAsync();
    }
}
