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

// Adiciona os serviços ao contêiner de injeção de dependência.
builder.Services.AddControllers();

// Adiciona o serviço do Swagger
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

// Registra as implementações de serviço
builder.Services.AddSingleton<IEventPublisher, RabbitMQPublisher>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

var app = builder.Build();

// Configura o middleware para a pipeline de requisição HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Logger.LogInformation("Serviço de Clientes iniciado.");

app.Run();