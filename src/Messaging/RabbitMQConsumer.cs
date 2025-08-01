using Core.Messaging;
using RabbitMQ.Client;

namespace Messaging;

public class RabbitMQConsumer : IEventConsumer
{
    public IChannel Channel { get; }
    private readonly IConnection _connection;

    public RabbitMQConsumer(IConnection connection, string queueName)
    {
        _connection = connection;

        // Cria o canal de forma assíncrona
        Channel = Task.Run(async () => await _connection.CreateChannelAsync()).Result;

        // Declara a fila de forma assíncrona
        Task.Run(async () => await Channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        )).Wait();
    }

    public void Ack(ulong deliveryTag)
    {
        Channel.BasicAckAsync(deliveryTag, false);
    }

    public void Nack(ulong deliveryTag)
    {
        Channel.BasicNackAsync(deliveryTag, false, false);
    }
}
