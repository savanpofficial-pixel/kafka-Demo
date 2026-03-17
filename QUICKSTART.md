# Kafka Demo - Quick Start Guide

This guide will help you get started with the refactored Kafka Producer and Consumer applications.

## Prerequisites

- **.NET 8.0 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Apache Kafka** running locally on `localhost:9092`
- **Terminal/Command Prompt** access

## Project Structure

```
kafka-Demo/
├── KafkaProducer/          # Producer application
├── KafkaConsumer/          # Consumer application
└── kafka-Demo.sln          # Solution file
```

## Getting Started

### ⚠️ IMPORTANT: Start Kafka Server First!

**Before running the .NET applications, you MUST have Kafka server running on `localhost:9092`**

#### Option 1: Using Docker (Easiest)

```bash
# Start Kafka with Docker
docker run -d --name kafka -p 9092:9092 apache/kafka:latest

# Verify Kafka is running
docker ps
```

#### Option 2: Using Local Kafka Installation

If you have Kafka installed locally:

```bash
# Start Zookeeper (if required by your Kafka version)
zookeeper-server-start.bat config\zookeeper.properties

# Start Kafka (in a new terminal)
kafka-server-start.bat config\server.properties
```

#### Verify Kafka is Running

```bash
# Windows - Check if port 9092 is listening
netstat -an | findstr "9092"

# You should see something like:
# TCP    0.0.0.0:9092           0.0.0.0:0              LISTENING
```

---

### Step 1: Navigate to Project Root

Open a terminal/command prompt and navigate to the project root:

```bash
cd C:\kafka-Demo
```

### Step 2: Restore NuGet Packages

Run this command from the project root to restore all dependencies:

```bash
dotnet restore
```

This will download all required NuGet packages for both projects:
- Confluent.Kafka
- Microsoft.Extensions.Hosting
- Microsoft.Extensions.Configuration.Json

### Step 3: Build the Solution

Build both projects:

```bash
dotnet build
```

If successful, you should see:
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

## Running the Applications

### 📋 Execution Order Summary

**IMPORTANT: Follow this order to avoid connection errors!**

1. ✅ **Start Kafka Server** (must be running first!)
2. ✅ **Start Consumer Application** (optional, but recommended)
3. ✅ **Start Producer Application**

If Kafka is not running, you'll see errors like:
```
Failed to deliver message: Local: Broker transport failure
```

---

### Option 1: Run Both Projects (Recommended)

**Terminal 1 - Start the Consumer:**
```bash
cd C:\kafka-Demo\KafkaConsumer
dotnet run
```

**Terminal 2 - Start the Producer:**
```bash
cd C:\kafka-Demo\KafkaProducer
dotnet run
```

### Option 2: Run from Solution Root

**Terminal 1 - Consumer:**
```bash
dotnet run --project C:\kafka-Demo\KafkaConsumer\KafkaConsumer.csproj
```

**Terminal 2 - Producer:**
```bash
dotnet run --project C:\kafka-Demo\KafkaProducer\KafkaProducer.csproj
```

## What to Expect

### Consumer Output:
```
info: KafkaConsumer.Services.KafkaConsumerService[0]
      Kafka Consumer initialized. Subscribed to topic: test, GroupId: dotnet-consumer-group
info: KafkaConsumer.Program[0]
      Starting Kafka Consumer. Press Ctrl+C to exit.
info: KafkaConsumer.Services.KafkaConsumerService[0]
      Kafka Consumer started. Listening to 'test' topic...
```

### Producer Output:
```
info: KafkaProducer.Services.KafkaProducerService[0]
      Kafka Producer initialized with bootstrap servers: localhost:9092
info: KafkaProducer.Program[0]
      Kafka Producer started. Press Ctrl+C to exit.
info: KafkaProducer.Services.KafkaProducerService[0]
      Sending message with key: message-key
info: KafkaProducer.Services.KafkaProducerService[0]
      Message delivered to test [0] @1. Status: Persisted, Partition: 0, Offset: 1
info: KafkaProducer.Program[0]
      Message sent successfully!
```

### Consumer Receiving Message:
```
info: KafkaConsumer.Handlers.MessageHandler[0]
      Message Received - Key: message-key, Value: Hello from .NET Kafka Producer, Partition: 0, Offset: 1, Timestamp: 3/17/2026 10:30:45 AM
```

## Configuration

Both applications use `appsettings.json` for configuration. You can modify settings without recompiling.

### KafkaProducer/appsettings.json:
```json
{
  "KafkaProducer": {
    "BootstrapServers": "localhost:9092",
    "ClientId": "dotnet-producer",
    "Topic": "test"
  }
}
```

### KafkaConsumer/appsettings.json:
```json
{
  "KafkaConsumer": {
    "BootstrapServers": "localhost:9092",
    "GroupId": "dotnet-consumer-group",
    "Topic": "test",
    "AutoOffsetReset": "Earliest",
    "EnableAutoCommit": true
  }
}
```

## Troubleshooting

### Issue: "Kafka broker not available"

**Solution:** Ensure Kafka is running on `localhost:9092`

```bash
# Check if Kafka is running (Windows)
netstat -an | findstr "9092"

# Start Kafka (example using Docker)
docker run -d --name kafka -p 9092:9092 apache/kafka:latest
```

### Issue: "Topic 'test' does not exist"

**Solution:** Create the topic manually or enable auto-creation

```bash
# Using Kafka CLI tools
kafka-topics --create --topic test --bootstrap-server localhost:9092 --partitions 1 --replication-factor 1
```

### Issue: Build errors

**Solution:**
1. Verify .NET 8.0 SDK is installed: `dotnet --version`
2. Clean and rebuild:
   ```bash
   dotnet clean
   dotnet restore
   dotnet build
   ```

### Issue: "No messages received in consumer"

**Checklist:**
1. Consumer must be running BEFORE producer sends messages (if using `AutoOffsetReset: Latest`)
2. Or set `AutoOffsetReset: Earliest` to consume from beginning
3. Verify both apps use the same topic name
4. Check Kafka broker is accessible

## Stopping the Applications

Press `Ctrl+C` in each terminal to gracefully shut down the applications.

You should see:
```
info: Program[0]
      Shutdown requested...
info: KafkaConsumer.Services.KafkaConsumerService[0]
      Consumer closed
```

## Next Steps

### Customize Message Handling

Edit `KafkaConsumer/Handlers/MessageHandler.cs` to add your business logic:

```csharp
public Task HandleMessageAsync(ConsumeResult<string, string> consumeResult, CancellationToken cancellationToken = default)
{
    _logger.LogInformation("Processing message: {Value}", consumeResult.Message.Value);

    // Add your business logic here
    // - Parse JSON
    // - Store in database
    // - Call external APIs
    // - etc.

    return Task.CompletedTask;
}
```

### Send Custom Messages

Modify `KafkaProducer/Program.cs` to send different messages:

```csharp
var success = await producerService.ProduceMessageAsync(
    "custom-key",
    "Your custom message here",
    cts.Token);
```

### Change Configuration

Update `appsettings.json` to:
- Connect to different Kafka brokers
- Use different topics
- Change consumer group IDs
- Modify logging levels

## Architecture Overview

### Producer Architecture:
```
Program.cs (entry point)
    ↓
IKafkaProducerService (interface)
    ↓
KafkaProducerService (Kafka logic)
    ↓
Confluent.Kafka Producer
```

### Consumer Architecture:
```
Program.cs (entry point)
    ↓
IKafkaConsumerService (interface)
    ↓
KafkaConsumerService (Kafka logic)
    ↓
IMessageHandler (interface)
    ↓
MessageHandler (business logic)
```

## Additional Resources

- [Confluent.Kafka Documentation](https://docs.confluent.io/kafka-clients/dotnet/current/overview.html)
- [Apache Kafka Documentation](https://kafka.apache.org/documentation/)
- [.NET Dependency Injection](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)
- [.NET Logging](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging)

## Support

For issues or questions:
1. Check the troubleshooting section above
2. Review application logs for detailed error messages
3. Verify Kafka broker connectivity
4. Check configuration in `appsettings.json`
