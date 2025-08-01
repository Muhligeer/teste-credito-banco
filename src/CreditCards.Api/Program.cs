using CreditCards.Api.BackgroundServices;
using CreditCards.Api.Services;
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

builder.Services.AddSingleton<IEventPublisher, RabbitMQPublisher>();
builder.Services.AddScoped<ICreditCardService, CreditCardService>();

builder.Services.AddSingleton(sp =>
{
    var connection = sp.GetRequiredService<IConnection>();
    return new RabbitMQConsumer(connection, "credit.proposal.queue");
});

builder.Services.AddHostedService<CreditProposalEventConsumerService>();

var app = builder.Build();

app.Run();