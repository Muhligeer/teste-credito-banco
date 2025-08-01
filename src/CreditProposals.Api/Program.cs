using System.Threading.Tasks;
using CreditProposals.Api.BackgroundServices;
using CreditProposals.Api.Services;
using Core.Messaging;
using Core.Services;
using Messaging;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IConnection>(sp =>
{
    var factory = new ConnectionFactory() { HostName = "rabbitmq" };
    return factory.CreateConnectionAsync().Result;
});

// Registra as implementações de serviço
builder.Services.AddSingleton<IEventPublisher, RabbitMQPublisher>();
builder.Services.AddScoped<ICreditProposalService, CreditProposalService>();

builder.Services.AddSingleton(sp =>
{
    var connection = sp.GetRequiredService<IConnection>();
    return new RabbitMQConsumer(connection, "client.created.queue");
});

builder.Services.AddHostedService<ClientCreatedEventConsumerService>();

var app = builder.Build();

app.Run();