namespace Core.Messaging;

public interface IEventPublisher
{
    Task PublishAsync<T>(T eventMessage, string exchangeName, string routingKey);
}
