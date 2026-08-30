using Testcontainers.RabbitMq;

namespace Template.Api.IntegrationTests.Messaging;

public sealed class RabbitMqTestFactory
    : IAsyncLifetime
{
    private readonly RabbitMqContainer _rabbitMq =
        new RabbitMqBuilder("rabbitmq:4-management")
            .WithUsername("template")
            .WithPassword("template")
            .Build();

    
    public string Host => _rabbitMq.Hostname;

    public int Port => _rabbitMq.GetMappedPublicPort(5672);

    public string Username => "template";

    public string Password => "template";

    public async Task InitializeAsync()
    {
        await _rabbitMq.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _rabbitMq.DisposeAsync();
    }
}