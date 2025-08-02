using Core.Services;
using Messaging;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Contracts.Events;
using Core.Configurations;
using Microsoft.Extensions.Options;

namespace CreditCards.Api.BackgroundServices;

public class CreditProposalEventConsumerService : BackgroundService
{
    private readonly RabbitMQConsumer _consumer;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CreditProposalEventConsumerService> _logger;

    public CreditProposalEventConsumerService(RabbitMQConsumer consumer, IServiceProvider serviceProvider, ILogger<CreditProposalEventConsumerService> logger)
    {
        _consumer = consumer;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        IChannel channel = _consumer.Channel;

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
                _logger.LogError(ex, "Falha ao processar mensagem.");
                _consumer.Reject(ea.DeliveryTag, false);
            }
        };

        channel.BasicConsumeAsync(queue: _consumer.QueueName, autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }
}
