namespace Contracts.DTOs;

public class CustomerResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Document { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public CustomerResponse(Guid id, string name, string document, string email)
    {
        Id = id;
        Name = name;
        Document = document;
        Email = email;
        CreatedAt = DateTime.UtcNow;
    }
}
