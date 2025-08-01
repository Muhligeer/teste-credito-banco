using Contracts.Events;
using Core.Messaging;
using Core.Services;

namespace CreditCards.Api.Services;

public class CreditCardService : ICreditCardService
{
    private readonly IEventPublisher _eventPublisher;

    public CreditCardService(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task IssueCreditCardAsync(CreditProposalCreatedEvent creditProposalEvent)
    {
        Console.WriteLine($"[INFO] Recebido evento de proposta de crédito para o cliente: {creditProposalEvent.ClientId}");

        if (creditProposalEvent.Status == "Approved")
        {
            var creditCardIssuedEvent = new CreditCardIssuedEvent(
                cardId: Guid.NewGuid(),
                clientId: creditProposalEvent.ClientId,
                cardNumber: new Random().Next(10000000, 99999999).ToString(),
                cardHolderName: "Guilherme Cruz"
            );

            // Publica o evento de cartão emitido
            await _eventPublisher.PublishAsync(creditCardIssuedEvent, "creditcards.exchange", "credit.card.issued");
            Console.WriteLine($"[INFO] Cartão emitido para o cliente {creditProposalEvent.ClientId}.");
        }
        else
        {
            Console.WriteLine($"[INFO] Proposta negada para o cliente {creditProposalEvent.ClientId}. Nenhum cartão foi emitido.");
        }
    }
}
