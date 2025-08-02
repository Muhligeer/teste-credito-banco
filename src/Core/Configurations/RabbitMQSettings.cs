namespace Core.Configurations;

public class RabbitMQSettings
{
    public required string HostName { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string CustomerExchange { get; set; }
    public required string ClientCreatedQueue { get; set; }
    public required string ClientCreatedRoutingKey { get; set; }
    public required string CreditProposalExchange { get; set; }
    public required string CreditProposalQueue { get; set; }
    public required string CreditProposalRoutingKey { get; set; }
    public required string CreditCardsExchange { get; set; }
    public required string CreditCardIssuedQueue { get; set; }
    public required string CreditCardIssuedRoutingKey { get; set; }
    public required string DeadLetterExchange { get; set; }
    public required string DeadLetterQueue { get; set; }
}
