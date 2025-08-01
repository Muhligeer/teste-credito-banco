using Core.Messaging;
using Core.Services;
using Messaging;
using RabbitMQ.Client;
using Customers.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Adiciona os serviços ao contêiner de injeção de dependência.
builder.Services.AddControllers();

// Adiciona o serviço do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IConnection>(sp =>
{
    var factory = new ConnectionFactory() { HostName = "rabbitmq" };
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

app.Run();