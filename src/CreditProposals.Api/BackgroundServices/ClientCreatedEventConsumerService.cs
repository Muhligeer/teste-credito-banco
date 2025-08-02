using Contracts.Events;
using Core.Services;
using CreditProposals.Api.Services;
using Messaging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using System.Text;
using Core.Configurations;
using Microsoft.Extensions.Options;

namespace CreditProposals.Api.BackgroundServices;

public class ClientCreatedEventConsumerService : BackgroundService
{
    private readonly RabbitMQConsumer _consumer;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ClientCreatedEventConsumerService> _logger;

    public ClientCreatedEventConsumerService(RabbitMQConsumer consumer, IServiceProvider serviceProvider, ILogger<ClientCreatedEventConsumerService> logger)
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
                _logger.LogError(ex, "Falha ao processar mensagem.");
                _consumer.Reject(ea.DeliveryTag, false);
            }
        };

        channel.BasicConsumeAsync(queue: _consumer.QueueName, autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }
}
