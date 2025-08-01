using Contracts.DTOs;
using Contracts.Events;
using Core.Messaging;
using Core.Models;
using Core.Services;

namespace Customers.Api.Services;

public class CustomerService : ICustomerService
{
    private readonly IEventPublisher _eventPublisher;

    public CustomerService(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public async Task<CustomerResponse> CreateCustomerAsync(CustomerRequest request)
    {
        // Simulação de persistência e lógica de negócio
        var customer = new Customer(Guid.NewGuid(), request.Name, request.Document, request.Email);

        // Cria o evento e o publica no RabbitMQ
        var clientCreatedEvent = new ClientCreatedEvent(
            clientId: customer.Id,
            name: customer.Name,
            document: customer.Document,
            email: customer.Email
        );

        await _eventPublisher.PublishAsync(clientCreatedEvent, "customers.exchange", "client.created");

        Console.WriteLine($"[INFO] Cliente {customer.Id} criado e evento publicado.");

        return new CustomerResponse(customer.Id, customer.Name, customer.Document, customer.Email);
    }
}
