using Confluent.Kafka;
using Learn.Kafka.Taxi.Shared.Interfaces;
using Learn.Kafka.Taxi.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Learn.Kafka.Taxi.Shared
{
    public class VehicleOutputTopicConsumer : IDisposable
    {
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly Action<string> _log;

        public VehicleOutputTopicConsumer(ConsumerConfig config, Action<string> log)
        {
            _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            _log = log;
        }

        public void Subscribe(string topic)
        {
            _consumer.Subscribe(topic);
        }

        public void Start(CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var result = _consumer.Consume(cancellationToken);
                        var messageResult = JsonSerializer.Deserialize<VehicleDistanceCalculatedResult>(result.Message.Value);
                        _log($"INFO: Received Message: {result.Message.Value}");
                    }
                    catch (Exception ex)
                    {
                        _log($"ERROR: {ex.Message}");
                    }
                }
                _consumer.Close();
            }, TaskCreationOptions.LongRunning);
        }

        public void Dispose() => _consumer.Dispose();
    }
}
