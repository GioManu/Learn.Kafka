using AutoFixture;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using DotNet.Testcontainers.Builders;
using Nito.AsyncEx;
using System.Net;
using System.Net.Sockets;

namespace Learn.Kafka.Containers.Test.Setups
{
    internal class TestContainersSetup : ICustomization
    {
        public async void Customize(IFixture fixture)
        {
            var zookeperContainerName = $"zookeeper_{fixture.Create<string>()}";
            var zookeperContainer = new ContainerBuilder()
                .WithImage("confluentinc/cp-zookeeper:latest")
                .WithName(zookeperContainerName)
                .WithEnvironment(new Dictionary<string, string>
                {
                    {"ZOOKEEPER_CLIENT_PORT", "2181"},
                    {"ZOOKEEPER_TICK_TIME", "200" }
                })
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(2181))
                .Build();

            AsyncContext.Run(async () => await zookeperContainer.StartAsync());

            var zookeperHostPort = zookeperContainer.GetMappedPublicPort(2181);

            var kafkaHostPort = GetAvailablePort();
            var kafkaContainerName = $"kafka_{fixture.Create<string>()}";
            var kafkaContainer = new ContainerBuilder()
                .WithImage("confluentinc/cp-kafka:latest")
                .WithName(kafkaContainerName)
                .WithHostname(kafkaContainerName)
                .WithPortBinding(kafkaHostPort, 9092)
                .WithEnvironment(new Dictionary<string, string>()
                {
                    { "KAFKA_BROKER_ID", "1" },
                    { "KAFKA_ZOOKEEPER_CONNECT", $"host.docker.internal:{zookeperHostPort}" },
                    { "KAFKA_LISTENER_SECURITY_PROTOCOL_MAP", "PLAINTEXT_INTERNAL:PLAINTEXT,EXTERNAL:PLAINTEXT,INTERNAL:PLAINTEXT" },
                    { "KAFKA_LISTENERS", "PLAINTEXT://:9092,PLAINTEXT_INTERNAL://:29092" },
                    { "KAFKA_ADVERTISED_LISTENERS", $"PLAINTEXT://localhost:{kafkaHostPort},PLAINTEXT_INTERNAL://{kafkaContainerName}:29092" },
                    { "KAFKA_INTER_BROKER_LISTENER_NAME", "PLAINTEXT_INTERNAL" },
                    { "KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR", "2" }
                })
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(9092))
                .Build();

            AsyncContext.Run(async () => await zookeperContainer.StartAsync());

            var inputTopic = $"input_{fixture.Create<string>()}";
            var outputTopic = $"output_{fixture.Create<string>()}";

            AsyncContext.Run(async () => await CreateKafkaTopic(inputTopic, $"localhost:{kafkaHostPort}", 2, 3));
            AsyncContext.Run(async () => await CreateKafkaTopic(outputTopic, $"localhost:{kafkaHostPort}", 2, 3));

            fixture.Inject(new KafkaTestConfig
            {
                InputTopic = inputTopic,
                OutputTopic = outputTopic,
                BootstrapServers = $"localhost:{kafkaHostPort}"
            });
        }

        private static int GetAvailablePort()
        {
            var defaultLoopbackEndpoint = new IPEndPoint(IPAddress.Loopback, 0);
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(defaultLoopbackEndpoint);

            return (socket.LocalEndPoint as IPEndPoint)!.Port;
        }

        private static async Task CreateKafkaTopic(string topicName, string bootStrapServers, short replicationFactor = 1, int partitions = 1)
        {
            using var adminClient = new AdminClientBuilder(new AdminClientConfig
            {
                BootstrapServers = bootStrapServers
            }).Build();
            await adminClient.CreateTopicsAsync(new TopicSpecification[]
            {
                new() { Name = topicName, ReplicationFactor = replicationFactor, NumPartitions = partitions }
            });
        }
    }
}
