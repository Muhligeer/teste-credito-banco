using Contracts.Events;
using Core.Services;
using CreditProposals.Api.Services;
using Messaging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using System.Text;

namespace CreditProposals.Api.BackgroundServices;

public class ClientCreatedEventConsumerService : BackgroundService
{
    private readonly RabbitMQConsumer _consumer;
    private readonly IServiceProvider _serviceProvider; // Injetar o provedor de serviços
    private readonly string _queueName = "client.created.queue";

    public ClientCreatedEventConsumerService(RabbitMQConsumer consumer, IServiceProvider serviceProvider)
    {
        _consumer = consumer;
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        IChannel channel = _consumer.Channel;
        var consumer = new AsyncEventingBasicConsumer(channel);


        channel.QueueBindAsync(
            queue: _queueName,
            exchange: "customers.exchange",
            routingKey: "client.created"
        ).Wait();

        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var clientCreatedEvent = JsonSerializer.Deserialize<ClientCreatedEvent>(message);

                if (clientCreatedEvent != null)
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var creditProposalService = scope.ServiceProvider.GetRequiredService<ICreditProposalService>();
                        await creditProposalService.ProcessCreditProposalAsync(clientCreatedEvent);
                    }
                }

                _consumer.Ack(ea.DeliveryTag);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO] Falha ao processar mensagem: {ex.Message}");
                _consumer.Nack(ea.DeliveryTag);
            }
        };

        channel.BasicConsumeAsync(queue: _queueName, autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }
}
