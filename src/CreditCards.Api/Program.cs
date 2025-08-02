using CreditCards.Api.BackgroundServices;
using CreditCards.Api.Services;
using Core.Messaging;
using Core.Services;
using Messaging;
using RabbitMQ.Client;
using Core.Configurations;
using Serilog;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddSingleton<IConnection>(sp =>
{
    var settings = builder.Configuration.GetSection("RabbitMQ").Get<RabbitMQSettings>();
    var factory = new ConnectionFactory()
    {
        HostName = settings.HostName,
        UserName = settings.Username,
        Password = settings.Password
    };
    return factory.CreateConnectionAsync().Result;
});

builder.Services.AddSingleton<IEventPublisher, RabbitMQPublisher>();
builder.Services.AddScoped<ICreditCardService, CreditCardService>();

builder.Services.AddSingleton(sp =>
{
    var settings = sp.GetRequiredService<IOptions<RabbitMQSettings>>().Value;

    var connection = sp.GetRequiredService<IConnection>();
    return new RabbitMQConsumer(
        connection, 
        settings.CreditProposalQueue,
        settings.CreditProposalExchange,
        settings.CreditProposalRoutingKey,
        settings.DeadLetterExchange,
        settings.DeadLetterQueue
    );
});

builder.Services.AddHostedService<CreditProposalEventConsumerService>();

var app = builder.Build();

app.Logger.LogInformation("Serviço de Cartões de Crédito iniciado.");
app.Run();