using Contracts.Events;
using Core.Configurations;
using Core.Messaging;
using Core.Services;
using Microsoft.Extensions.Options;

namespace CreditProposals.Api.Services;

public class CreditProposalService : ICreditProposalService
{
    private readonly IEventPublisher _eventPublisher;
    private readonly RabbitMQSettings _rabbitMQSettings;
    private readonly ILogger<CreditProposalService> _logger;

    public CreditProposalService(IEventPublisher eventPublisher, IOptions<RabbitMQSettings> settings, ILogger<CreditProposalService> logger)
    {
        _eventPublisher = eventPublisher;
        _rabbitMQSettings = settings.Value;
        _logger = logger;
    }

    public async Task ProcessCreditProposalAsync(ClientCreatedEvent clientCreatedEvent)
    {
        _logger.LogInformation("[INFO] Recebido evento para cliente: {ClientId}", clientCreatedEvent.ClientId);

        var isApproved = new Random().Next(0, 10) > 3;
        var creditLimit = isApproved ? (decimal)new Random().Next(1000, 10000) : 0;
        var status = isApproved ? "Approved" : "Denied";

        var creditProposalEvent = new CreditProposalCreatedEvent(
            creditProposalId: Guid.NewGuid(),
            clientId: clientCreatedEvent.ClientId,
            creditLimit: creditLimit,
            status: status
        );

        await _eventPublisher.PublishAsync(creditProposalEvent, _rabbitMQSettings.CreditProposalExchange, _rabbitMQSettings.CreditProposalRoutingKey);
        _logger.LogInformation("[INFO] Proposta de crédito gerada para o cliente {ClientId}. Status: {Status}", clientCreatedEvent.ClientId, status);
    }
}
