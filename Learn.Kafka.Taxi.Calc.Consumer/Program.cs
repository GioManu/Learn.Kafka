using Confluent.Kafka;
using Learn.Kafka.Taxi.Shared;

Console.WriteLine("Hello From Calc Consumer");

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
var messageHandler = new KafkaInputTopicMessageHandler(new VehicleCoordsProducerService(), new DistanceCalculatorService());
using var consumer = new VehicleCoordsConsumer(consumerConfig, Console.WriteLine, messageHandler);
consumer.Subscribe(KafkaSettings.InputTopic);

consumer.Start(cancellationToken);

if(Console.ReadKey().Key == ConsoleKey.Enter)
{
    cancellationTokenSource.Cancel();
}

Console.WriteLine("Consumer Shut-Down");
Console.ReadLine();