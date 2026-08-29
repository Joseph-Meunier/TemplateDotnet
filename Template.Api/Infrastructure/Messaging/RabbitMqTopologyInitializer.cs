using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Template.Shared.Events;

namespace Template.Api.Infrastructure.Messaging;

public sealed class RabbitMqTopologyInitializer(
    RabbitMqConnection connection,
    IOptions<RabbitMqOptions> options,
    IEnumerable<IMessageTopology> topologies)
{
    private readonly RabbitMqOptions _options = options.Value;

    public async Task InitializeAsync(
        CancellationToken cancellationToken)
    {
        const string deadLetterExchange = "template.dead-letter";

        var rabbitConnection =
            await connection.GetConnectionAsync(
                cancellationToken);

        await using var channel =
            await rabbitConnection.CreateChannelAsync(
                cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(
            exchange: _options.Exchange,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(
            exchange: deadLetterExchange,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        foreach (var topology in topologies)
        {
            foreach (var subscription in topology.Subscriptions)
            {
                await channel.QueueDeclareAsync(
                    queue: subscription.DeadLetterQueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    cancellationToken: cancellationToken);

                await channel.QueueBindAsync(
                    queue: subscription.DeadLetterQueueName,
                    exchange: deadLetterExchange,
                    routingKey: subscription.QueueName,
                    cancellationToken: cancellationToken);

                var arguments = new Dictionary<string, object?>
                {
                    ["x-dead-letter-exchange"] = deadLetterExchange,
                    ["x-dead-letter-routing-key"] = subscription.QueueName
                };

                await channel.QueueDeclareAsync(
                    queue: subscription.QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: arguments,
                    cancellationToken: cancellationToken);

                await channel.QueueBindAsync(
                    queue: subscription.QueueName,
                    exchange: _options.Exchange,
                    routingKey: subscription.RoutingKey,
                    cancellationToken: cancellationToken);
            }
        }
    }
}