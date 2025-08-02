using Contracts.Events;
using Core.Configurations;
using Core.Messaging;
using Core.Services;
using Microsoft.Extensions.Options;

namespace CreditCards.Api.Services;

public class CreditCardService : ICreditCardService
{
    private readonly IEventPublisher _eventPublisher;
    private readonly RabbitMQSettings _rabbitMQSettings;
    private readonly ILogger<CreditCardService> _logger;

    public CreditCardService(IEventPublisher eventPublisher, IOptions<RabbitMQSettings> settings, ILogger<CreditCardService> logger)
    {
        _eventPublisher = eventPublisher;
        _rabbitMQSettings = settings.Value;
        _logger = logger;
    }

    public async Task IssueCreditCardAsync(CreditProposalCreatedEvent creditProposalEvent)
    {
        _logger.LogInformation("[INFO] Recebido evento de proposta de crédito para o cliente: {ClientId}.", creditProposalEvent.ClientId);

        if (creditProposalEvent.Status == "Approved")
        {
            var creditCardIssuedEvent = new CreditCardIssuedEvent(
                cardId: Guid.NewGuid(),
                clientId: creditProposalEvent.ClientId,
                cardNumber: new Random().Next(10000000, 99999999).ToString(),
                cardHolderName: "Guilherme Cruz"
            );

            // Publica o evento de cartão emitido
            await _eventPublisher.PublishAsync(creditCardIssuedEvent, _rabbitMQSettings.CreditCardsExchange, _rabbitMQSettings.CreditCardIssuedRoutingKey);
            _logger.LogInformation("[INFO] Cartão emitido para o cliente {ClientId}.", creditProposalEvent.ClientId);
        }
        else
        {
            _logger.LogInformation("[INFO] Proposta negada para o cliente {ClientId}.", creditProposalEvent.ClientId);
        }
    }
}
