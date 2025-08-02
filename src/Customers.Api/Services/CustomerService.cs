using Contracts.DTOs;
using Contracts.Events;
using Core.Configurations;
using Core.Messaging;
using Core.Models;
using Core.Services;
using Microsoft.Extensions.Options;

namespace Customers.Api.Services;

public class CustomerService : ICustomerService
{
    private readonly IEventPublisher _eventPublisher;
    private readonly RabbitMQSettings _rabbitMQSettings;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(IEventPublisher eventPublisher, IOptions<RabbitMQSettings> settings, ILogger<CustomerService> logger)
    {
        _eventPublisher = eventPublisher;
        _rabbitMQSettings = settings.Value;
        _logger = logger;
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

        await _eventPublisher.PublishAsync(clientCreatedEvent, _rabbitMQSettings.CustomerExchange, _rabbitMQSettings.ClientCreatedRoutingKey);

        _logger.LogInformation("[INFO] Cliente {CustomerId} criado e evento publicado.", customer.Id);

        return new CustomerResponse(customer.Id, customer.Name, customer.Document, customer.Email);
    }
}
