namespace Learn.Kafka.Taxi.Shared.Interfaces
{
    public interface IVehicleProducer
    {
        Task ProduceAsync(string topic, Guid key, string message, string messageType);
    }
}
