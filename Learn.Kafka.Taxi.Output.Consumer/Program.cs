using Confluent.Kafka;
using Learn.Kafka.Taxi.Shared;

Console.WriteLine("Hello From Output Consumer");

var consumerConfig = new ConsumerConfig
{
    BootstrapServers = KafkaSettings.BootstrapServers,
    GroupId = KafkaSettings.CalcConsumerGroupId,
    AutoOffsetReset = AutoOffsetReset.Earliest,
    EnableAutoCommit = true,
    AutoCommitIntervalMs = 101,
};

var cancellationTokenSource = new CancellationTokenSource();
var cancellationToken = cancellationTokenSource.Token;
using var consumer = new VehicleOutputTopicConsumer(consumerConfig, Console.WriteLine);
consumer.Subscribe(KafkaSettings.OutputTopic);

consumer.Start(cancellationToken);

if (Console.ReadKey().Key == ConsoleKey.Enter)
{
    cancellationTokenSource.Cancel();
}

Console.ReadLine();