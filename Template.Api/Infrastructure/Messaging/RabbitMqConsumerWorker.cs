using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Template.Shared.Events;

namespace Template.Api.Infrastructure.Messaging;

public sealed class RabbitMqConsumerWorker(
    RabbitMqConnection rabbitMqConnection,
    IOptions<RabbitMqOptions> options,
    IEnumerable<IMessageTopology> topologies,
    IServiceScopeFactory scopeFactory,
    ILogger<RabbitMqConsumerWorker> logger)
    : BackgroundService
{
    private readonly RabbitMqOptions _options = options.Value;

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var connection =
            await rabbitMqConnection.GetConnectionAsync(stoppingToken);

        await using var channel =
            await connection.CreateChannelAsync(
                cancellationToken: stoppingToken);

        await channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 10,
            global: false,
            cancellationToken: stoppingToken);

        foreach (var subscription in topologies.SelectMany(x => x.Subscriptions))
        {
            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (_, args) =>
            {
                await HandleMessageAsync(
                    channel,
                    args,
                    stoppingToken);
            };

            await channel.BasicConsumeAsync(
                queue: subscription.QueueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);
        }

        await Task.Delay(
            Timeout.Infinite,
            stoppingToken);
    }

    private async Task HandleMessageAsync(
        IChannel channel,
        BasicDeliverEventArgs args,
        CancellationToken cancellationToken)
    {
        var eventType = args.BasicProperties.Type;

        if (string.IsNullOrWhiteSpace(eventType))
        {
            await channel.BasicNackAsync(
                args.DeliveryTag,
                multiple: false,
                requeue: false,
                cancellationToken);

            return;
        }

        var payload = Encoding.UTF8.GetString(
            args.Body.Span);

        await using var scope =
            scopeFactory.CreateAsyncScope();

        var handlers = scope.ServiceProvider
            .GetServices<IIntegrationEventHandler>();

        var handler = handlers.SingleOrDefault(
            x => x.EventType == eventType);

        if (handler is null)
        {
            logger.LogError(
                "No integration event handler registered for {EventType}.",
                eventType);

            await channel.BasicNackAsync(
                args.DeliveryTag,
                multiple: false,
                requeue: false,
                cancellationToken);

            return;
        }
        
        var rawMessageId = args.BasicProperties.MessageId;

        if (!Guid.TryParse(rawMessageId, out var messageId))
        {
            logger.LogError(
                "Integration event {EventType} has an invalid MessageId: {MessageId}.",
                eventType,
                rawMessageId);

            await channel.BasicNackAsync(
                args.DeliveryTag,
                multiple: false,
                requeue: false,
                cancellationToken);

            return;
        }

        try
        {
            await handler.HandleAsync(
                messageId,
                payload,
                cancellationToken);

            await channel.BasicAckAsync(
                args.DeliveryTag,
                multiple: false,
                cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Error processing integration event {EventType}.",
                eventType);

            await channel.BasicNackAsync(
                args.DeliveryTag,
                multiple: false,
                requeue: false,
                cancellationToken);
        }
    }
}