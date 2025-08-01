using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Events;

public class CreditProposalCreatedEvent
{
    public Guid CreditProposalId { get; set; }
    public Guid ClientId { get; set; }
    public decimal CreditLimit { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public CreditProposalCreatedEvent(Guid creditProposalId, Guid clientId, decimal creditLimit, string status)
    {
        CreditProposalId = creditProposalId;
        ClientId = clientId;
        CreditLimit = creditLimit;
        Status = status;
        CreatedAt = DateTime.UtcNow;
    }
}
