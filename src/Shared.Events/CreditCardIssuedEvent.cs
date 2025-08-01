using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Events;

public class CreditCardIssuedEvent
{
    public Guid CardId { get; set; }
    public Guid ClientId { get; set; }
    public string CardNumber { get; set; }
    public string CardHolderName { get; set; }

    public CreditCardIssuedEvent(Guid cardId, Guid clientId, string cardNumber, string cardHolderName)
    {
        CardId = cardId;
        ClientId = clientId;
        CardNumber = cardNumber;
        CardHolderName = cardHolderName;
    }
}
