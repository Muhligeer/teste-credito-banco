using Core.Messaging;
using RabbitMQ.Client;
using System.Data.Common;

namespace Messaging;

public class RabbitMQConsumer : IEventConsumer
{
    public IChannel Channel { get; }
    public string QueueName { get; }

    public RabbitMQConsumer(IConnection connection, string queueName, string exchangeName, string routingKey, string deadLetterExchangeName, string deadLetterQueueName)
    {
        Channel = Task.Run(async () => await connection.CreateChannelAsync()).Result;
        QueueName = queueName;

        Task.Run(async () => await Channel.ExchangeDeclareAsync(exchangeName, type: "topic", durable: true)).Wait();

        Task.Run(async () => await Channel.ExchangeDeclareAsync(deadLetterExchangeName, type: "fanout", durable: true)).Wait();
        Task.Run(async () => await Channel.QueueDeclareAsync(deadLetterQueueName, durable: true, exclusive: false, autoDelete: false)).Wait();
        Task.Run(async () => await Channel.QueueBindAsync(deadLetterQueueName, deadLetterExchangeName, "")).Wait();

        var queueArgs = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", deadLetterExchangeName }
            };
        Task.Run(async () => await Channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: queueArgs)).Wait();

        Task.Run(async () => await Channel.QueueBindAsync(queueName, exchangeName, routingKey)).Wait();
    }

    public void Ack(ulong deliveryTag)
    {
        Channel.BasicAckAsync(deliveryTag, false);
    }

    public void Reject(ulong deliveryTag, bool requeue)
    {
        Channel.BasicRejectAsync(deliveryTag, requeue);
    }
}
