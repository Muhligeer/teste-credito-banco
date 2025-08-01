using Core.Services;
using Messaging;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Contracts.Events;

namespace CreditCards.Api.BackgroundServices;

public class CreditProposalEventConsumerService : BackgroundService
{
    private readonly RabbitMQConsumer _consumer;
    private readonly IServiceProvider _serviceProvider;
    private readonly string _queueName = "credit.proposal.queue";

    public CreditProposalEventConsumerService(RabbitMQConsumer consumer, IServiceProvider serviceProvider)
    {
        _consumer = consumer;
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        IChannel channel = _consumer.Channel;

        channel.QueueBindAsync(
            queue: _queueName,
            exchange: "creditproposals.exchange",
            routingKey: "credit.proposal.created"
        ).Wait();

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var creditProposalEvent = JsonSerializer.Deserialize<CreditProposalCreatedEvent>(message);

                if (creditProposalEvent != null)
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var creditCardService = scope.ServiceProvider.GetRequiredService<ICreditCardService>();
                        await creditCardService.IssueCreditCardAsync(creditProposalEvent);
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
