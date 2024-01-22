namespace Learn.Kafka.Taxi.Shared.Interfaces
{
    public interface IMessageHandler<T>
    {
        Task Handle(T message);
    }
}
