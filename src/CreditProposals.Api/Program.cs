using System.Threading.Tasks;
using CreditProposals.Api.BackgroundServices;
using CreditProposals.Api.Services;
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

// Registra as implementações de serviço
builder.Services.AddSingleton<IEventPublisher, RabbitMQPublisher>();
builder.Services.AddScoped<ICreditProposalService, CreditProposalService>();

builder.Services.AddSingleton(sp =>
{
    var settings = sp.GetRequiredService<IOptions<RabbitMQSettings>>().Value;

    var connection = sp.GetRequiredService<IConnection>();
    return new RabbitMQConsumer(
        connection, 
        settings.ClientCreatedQueue,
        settings.CustomerExchange,
        settings.ClientCreatedRoutingKey,
        settings.DeadLetterExchange,
        settings.DeadLetterQueue
    );
});

builder.Services.AddHostedService<ClientCreatedEventConsumerService>();

var app = builder.Build();

app.Logger.LogInformation("Serviço de Propostas de Crédito iniciado.");
app.Run();