using Contracts.Events;
using Core.Messaging;
using Core.Services;

namespace CreditProposals.Api.Services;

public class CreditProposalService : ICreditProposalService
{
    private readonly IEventPublisher _eventPublisher;

    public CreditProposalService(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task ProcessCreditProposalAsync(ClientCreatedEvent clientCreatedEvent)
    {
        Console.WriteLine($"[INFO] Recebido evento para cliente: {clientCreatedEvent.ClientId}");

        var isApproved = new Random().Next(0, 10) > 3;
        var creditLimit = isApproved ? (decimal)new Random().Next(1000, 10000) : 0;
        var status = isApproved ? "Approved" : "Denied";

        var creditProposalEvent = new CreditProposalCreatedEvent(
            creditProposalId: Guid.NewGuid(),
            clientId: clientCreatedEvent.ClientId,
            creditLimit: creditLimit,
            status: status
        );

        await _eventPublisher.PublishAsync(creditProposalEvent, "creditproposals.exchange", "credit.proposal.created");
        Console.WriteLine($"[INFO] Proposta de crédito gerada para o cliente {clientCreatedEvent.ClientId}. Status: {status}");
    }
}
