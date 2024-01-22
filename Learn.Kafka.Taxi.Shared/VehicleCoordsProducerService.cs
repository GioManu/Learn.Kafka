using Confluent.Kafka;
using Learn.Kafka.Taxi.Shared.Interfaces;
using Learn.Kafka.Taxi.Shared.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learn.Kafka.Taxi.Shared
{
    public class VehicleCoordsProducerService : IVehicleProducer
    {
        private readonly IProducer<string, string> _producer;

        public VehicleCoordsProducerService()
        {
            var producerconfig = new ProducerConfig
            {
                BootstrapServers = KafkaSettings.BootstrapServers,
                AllowAutoCreateTopics = false,
                Acks = Acks.Leader
            };

            _producer = new ProducerBuilder<string, string>(producerconfig)
                .Build();
        }

        public async Task ProduceAsync(string topic, Guid key, string message, string messageType)
        {
            var headers = new Headers
            {
                new Header("type", Encoding.ASCII.GetBytes(messageType))
            };

            var kafkamessage = new Message<string, string>
            {
                Key = key.ToString(),
                Value = message,
                Headers = headers
            };

            await _producer.ProduceAsync(topic, kafkamessage);
        }
    }
}
