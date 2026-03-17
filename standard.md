You are a senior .NET engineer performing a code review.

Analyze my .NET console Kafka project and evaluate whether it follows good .NET development standards and clean architecture practices.

Project context:

* This is a .NET console application.
* It contains two projects: KafkaProducer and KafkaConsumer.
* Kafka is implemented using the Confluent.Kafka library.
* Most logic is currently inside Program.cs.

Please review the code and provide feedback on the following:

1. .NET coding standards

   * Naming conventions (PascalCase, camelCase, interfaces with I prefix)
   * Async method naming
   * Code readability

2. SOLID principles

   * Check if Single Responsibility Principle is violated
   * Suggest how to restructure if needed

3. Project structure

   * Evaluate if the current folder structure is appropriate for a console app
   * Suggest an improved structure if the project grows

4. Best practices

   * Dependency injection
   * Logging vs Console.WriteLine
   * Exception handling
   * Configuration management

5. Provide a recommended improved folder structure for this Kafka console project.

6. If improvements are needed, show example refactored code.

Keep suggestions practical for a small Kafka demo project, not a large enterprise application.
