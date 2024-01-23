using AutoFixture;
using Confluent.Kafka;
using FluentAssertions;
using Learn.Kafka.Containers.Test.Setups;
using Learn.Kafka.Taxi.Shared;
using Learn.Kafka.Taxi.Shared.Models;
using System.Text.Json;
using static Confluent.Kafka.ConfigPropertyNames;

namespace Learn.Kafka.Containers.Test
{
    public class KafkaTests
    {
        private readonly IFixture _fixture;

        public KafkaTests()
        {
            _fixture = new Fixture()
                .Customize(new TestContainersSetup());
        }

        [Fact]
        public async Task KAFKA_INPUT_TOPIC_CONSUMER_RECEIVED_MESSAGE_SUCCESS()
        {
            var vehicleRequest = new VehicleSignalRequest(Guid.NewGuid(), new Coords(1, 10));
            var testMessage = new Message<string, string> { Key = vehicleRequest.Id.ToString(), Value = JsonSerializer.Serialize(vehicleRequest) };
            var kafkaConfig = _fixture.Create<KafkaTestConfig>();
            var consumer = this.CreateConsumer(kafkaConfig.InputTopic, Guid.NewGuid().ToString(), kafkaConfig);
            var producer = this.CreateProducer(kafkaConfig);

            var deliveryReport = await producer.ProduceAsync(kafkaConfig.InputTopic, testMessage);
            Console.WriteLine($"Produced message to: {deliveryReport.TopicPartitionOffset}");

            await Task.Delay(TimeSpan.FromSeconds(5));

            var consumerResult = consumer.Consume(TimeSpan.FromSeconds(5));
            var consumerMessageResult = JsonSerializer.Deserialize<VehicleSignalRequest>(consumerResult.Message.Value);

            vehicleRequest.Should().BeEquivalentTo(consumerMessageResult);
        }

        private IConsumer<Null, string> CreateConsumer(string topicName, string groupId, KafkaTestConfig kafkaConfig)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = kafkaConfig.BootstrapServers,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            var consumer = new ConsumerBuilder<Null, string>(config).Build();
            consumer.Subscribe(topicName);

            return consumer;
        }
        
        private IProducer<string, string> CreateProducer(KafkaTestConfig kafkaConfig)
        {
            var producerconfig = new ProducerConfig
            {
                BootstrapServers = kafkaConfig.BootstrapServers,
                AllowAutoCreateTopics = false,
                Acks = Acks.Leader
            };

            return new ProducerBuilder<string, string>(producerconfig)
                .Build();
        }
    }
}