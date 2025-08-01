using Contracts.Events;

namespace Core.Services;

public interface ICreditProposalService
{
    Task ProcessCreditProposalAsync(ClientCreatedEvent clientCreatedEvent);
}
