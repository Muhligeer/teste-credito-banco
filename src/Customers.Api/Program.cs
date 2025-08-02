using Core.Messaging;
using Core.Services;
using Messaging;
using RabbitMQ.Client;
using Customers.Api.Services;
using Core.Configurations;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

// Adiciona os servi�os ao cont�iner de inje��o de depend�ncia.
builder.Services.AddControllers();

// Adiciona o servi�o do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Registra as implementa��es de servi�o
builder.Services.AddSingleton<IEventPublisher, RabbitMQPublisher>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

var app = builder.Build();

// Configura o middleware para a pipeline de requisi��o HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Logger.LogInformation("Servi�o de Clientes iniciado.");

app.Run();