using Confluent.Kafka;
using Learn.Kafka.Taxi.Shared.Interfaces;
using Learn.Kafka.Taxi.Shared.Models;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Learn.Kafka.Taxi.Shared
{
    public class VehicleCoordsConsumer : IDisposable
    {
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly Action<string> _log;
        private readonly IMessageHandler<VehicleSignalRequest> _handler;

        public VehicleCoordsConsumer(ConsumerConfig config, Action<string> log, IMessageHandler<VehicleSignalRequest> handler)
        {
            _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            _log = log;
            _handler = handler;
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
                        var messageResult = JsonSerializer.Deserialize<VehicleSignalRequest>(result.Message.Value);
                        _handler.Handle(messageResult);
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
