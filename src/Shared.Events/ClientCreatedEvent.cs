using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Events;

public class ClientCreatedEvent
{
    public Guid ClientId { get; set; }
    public string Name { get; set; }
    public string Document { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public ClientCreatedEvent(Guid clientId, string name, string document, string email)
    {
        ClientId = clientId;
        Name = name;
        Document = document;
        Email = email;
        CreatedAt = DateTime.UtcNow;
    }
}
