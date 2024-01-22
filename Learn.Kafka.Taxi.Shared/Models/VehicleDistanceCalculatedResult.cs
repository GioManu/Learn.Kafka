
namespace Learn.Kafka.Taxi.Shared.Models
{
    public class VehicleDistanceCalculatedResult(Guid vehicleId, double distance)
    {
        public Guid VehicleId { get; set; } = vehicleId;
        public double Distance { get; set; } = distance;
    }
}
