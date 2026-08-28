using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Template.Shared.Events;

namespace Template.Api.Infrastructure.Messaging;

public sealed class RabbitMqIntegrationEventPublisher(
    RabbitMqConnection rabbitMqConnection,
    IOptions<RabbitMqOptions> options)
    : IIntegrationEventPublisher
{
    private readonly RabbitMqOptions _options = options.Value;
    
    public async Task PublishAsync(
        Guid messageId,
        string type,
        string payload,
        CancellationToken cancellationToken)
    {
        var connection =
            await rabbitMqConnection.GetConnectionAsync(
                cancellationToken);

        await using var channel =
            await connection.CreateChannelAsync(
                cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(
            exchange: _options.Exchange,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        var body = Encoding.UTF8.GetBytes(payload);

        var properties = new BasicProperties
        {
            MessageId = messageId.ToString(),
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent,
            Type = type
        };

        await channel.BasicPublishAsync(
            exchange: _options.Exchange,
            routingKey: type,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);
    }
}