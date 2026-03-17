# Kafka Demo - C# .NET 8 Console Application

This project demonstrates Apache Kafka integration with C# .NET 8 using the Confluent.Kafka NuGet package.

## Project Structure

```
kafka-Demo/
├── KafkaProducer/
│   ├── KafkaProducer.csproj
│   └── Program.cs
├── KafkaConsumer/
│   ├── KafkaConsumer.csproj
│   └── Program.cs
└── README.md
```

## Prerequisites

1. **.NET 8 SDK** - Download from [https://dotnet.microsoft.com/download/dotnet/8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
2. **Apache Kafka** - Running locally on `localhost:9092`
3. **Kafka Topic** - A topic named `test` should be created

### Setting up Kafka (Quick Start)

If you don't have Kafka running, here's how to set it up:

#### Using Docker (Recommended)

```bash
# Start Kafka with Docker Compose
docker run -d --name zookeeper -p 2181:2181 -e ZOOKEEPER_CLIENT_PORT=2181 confluentinc/cp-zookeeper:latest
docker run -d --name kafka -p 9092:9092 -e KAFKA_ZOOKEEPER_CONNECT=localhost:2181 -e KAFKA_ADVERTISED_LISTENERS=PLAINTEXT://localhost:9092 -e KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR=1 confluentinc/cp-kafka:latest

# Create the 'test' topic
docker exec -it kafka kafka-topics --create --topic test --bootstrap-server localhost:9092 --partitions 1 --replication-factor 1
```

#### Manual Kafka Setup

```bash
# Download and extract Kafka
# Then start Zookeeper
bin/zookeeper-server-start.sh config/zookeeper.properties

# Start Kafka Server (in another terminal)
bin/kafka-server-start.sh config/server.properties

# Create topic
bin/kafka-topics.sh --create --topic test --bootstrap-server localhost:9092 --partitions 1 --replication-factor 1
```

## Installation Steps

### 1. Restore NuGet Packages

Navigate to each project folder and restore dependencies:

```bash
# Restore Producer dependencies
cd KafkaProducer
dotnet restore

# Restore Consumer dependencies
cd ../KafkaConsumer
dotnet restore
```

### 2. Build Projects

```bash
# Build Producer
cd KafkaProducer
dotnet build

# Build Consumer
cd ../KafkaConsumer
dotnet build
```

## Running the Applications

You need to run both the Consumer and Producer in separate terminal windows.

### Step 1: Start the Consumer (Terminal 1)

The consumer should be started first so it can listen for incoming messages:

```bash
cd KafkaConsumer
dotnet run
```

Expected output:
```
Kafka Consumer started. Listening to 'test' topic...
Press Ctrl+C to exit.
```

### Step 2: Start the Producer (Terminal 2)

In a new terminal window, run the producer to send a message:

```bash
cd KafkaProducer
dotnet run
```

Expected output:
```
Kafka Producer started. Press Ctrl+C to exit.
Sending message to Kafka...
Message delivered to test [0] @0
Status: Persisted
Partition: 0
Offset: 0

Message sent successfully!
```

### Step 3: Verify Message Reception

Check the Consumer terminal. You should see:

```
===== Message Received =====
Key: message-key
Value: Hello from .NET Kafka Producer
Partition: 0
Offset: 0
Timestamp: 2024-01-15 10:30:45
============================
```

## Code Overview

### Producer (KafkaProducer/Program.cs)

The producer creates a message with:
- **Key**: "message-key"
- **Value**: "Hello from .NET Kafka Producer"
- **Topic**: "test"

Key configuration:
- `BootstrapServers`: Points to Kafka broker at localhost:9092
- `ClientId`: Identifies the producer client

### Consumer (KafkaConsumer/Program.cs)

The consumer listens continuously for messages on the "test" topic.

Key configuration:
- `BootstrapServers`: Points to Kafka broker at localhost:9092
- `GroupId`: Consumer group identifier
- `AutoOffsetReset`: Set to `Earliest` to read from the beginning of the topic
- `EnableAutoCommit`: Automatically commits offsets

## Troubleshooting

### Connection Refused Error

If you see "Connection refused" errors:
1. Verify Kafka is running: `netstat -an | findstr 9092` (Windows) or `lsof -i :9092` (Mac/Linux)
2. Check Kafka logs for errors
3. Ensure the topic "test" exists

### No Messages Received

If the consumer doesn't receive messages:
1. Ensure the consumer is started before the producer
2. Check that both are using the same topic name ("test")
3. Verify Kafka broker address is correct (localhost:9092)

## NuGet Package Used

- **Confluent.Kafka** (v2.3.0) - Official .NET client for Apache Kafka

## Additional Resources

- [Confluent.Kafka Documentation](https://docs.confluent.io/kafka-clients/dotnet/current/overview.html)
- [Apache Kafka Documentation](https://kafka.apache.org/documentation/)
- [.NET 8 Documentation](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8)
