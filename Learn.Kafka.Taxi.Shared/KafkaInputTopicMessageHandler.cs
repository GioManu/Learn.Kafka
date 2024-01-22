using Confluent.Kafka;
using Learn.Kafka.Taxi.Shared.Interfaces;
using Learn.Kafka.Taxi.Shared.Models;
using System.Text.Json;

namespace Learn.Kafka.Taxi.Shared
{
    public class KafkaInputTopicMessageHandler : IMessageHandler<VehicleSignalRequest>
    {
        private readonly IVehicleProducer _producer;
        private readonly IDistanceCalculatorService _distanceCalculatorService;

        public KafkaInputTopicMessageHandler(IVehicleProducer producer, IDistanceCalculatorService distanceCalculatorService)
        {
            _producer = producer;
            _distanceCalculatorService = distanceCalculatorService;
        }

        public async Task Handle(VehicleSignalRequest request)
        {
            var calculatedResults = CalculateVehicleDistance(request);
            await _producer.ProduceAsync(KafkaSettings.OutputTopic, Guid.NewGuid(), JsonSerializer.Serialize(calculatedResults), nameof(VehicleDistanceCalculatedResult));
        }

        private VehicleDistanceCalculatedResult CalculateVehicleDistance(VehicleSignalRequest request)
        {
            var calculatedDistance = _distanceCalculatorService.CalculateDistance(request.Id, request.Coords);
            return new VehicleDistanceCalculatedResult(request.Id, calculatedDistance);
        }
    }
}
