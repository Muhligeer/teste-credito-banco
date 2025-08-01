using Core.Messaging;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

namespace Messaging;

public class RabbitMQPublisher : IEventPublisher
{
    private readonly IConnection _connection;

    public RabbitMQPublisher(IConnection connection)
    {
        _connection = connection;
    }

    public async Task PublishAsync<T>(T @event, string exchangeName, string routingKey)
    {
        using var channel = await _connection.CreateChannelAsync();

        // Adicione esta linha para declarar a exchange, garantindo que ela exista antes de publicar
        await channel.ExchangeDeclareAsync(exchangeName, type: "topic", durable: true);

        var message = JsonSerializer.Serialize(@event);
        var body = System.Text.Encoding.UTF8.GetBytes(message);

        await channel.BasicPublishAsync(
            exchange: exchangeName,
            routingKey: routingKey,
            body: body
        );
    }

}
