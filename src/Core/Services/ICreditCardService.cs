using Contracts.Events;

namespace Core.Services;

public interface ICreditCardService
{
    Task IssueCreditCardAsync(CreditProposalCreatedEvent creditProposalEvent);
}
