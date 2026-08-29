using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Template.Api.Infrastructure.Messaging;
using Template.Modules.Blog.Contracts.Events;

namespace Template.Api.IntegrationTests.Messaging;

public sealed class RabbitMqPublisherTests(
    RabbitMqTestFactory factory)
    : IClassFixture<RabbitMqTestFactory>
{
    [Fact]
    public async Task PublishAsync_PublishesMessageToRabbitMq()
    {
        const string exchangeName = "test.events";
        const string queueName = "test.blog.post-published";

        var options = Options.Create(
            new RabbitMqOptions
            {
                Host = factory.Host,
                Port = factory.Port,
                Username = factory.Username,
                Password = factory.Password,
                Exchange = exchangeName
            });

        await using var connection =
            new RabbitMqConnection(options);

        var publisher =
            new RabbitMqIntegrationEventPublisher(
                connection,
                options);

        var rabbitConnection =
            await connection.GetConnectionAsync(
                CancellationToken.None);

        await using var channel =
            await rabbitConnection.CreateChannelAsync();

        var routingKey =
            typeof(PostPublishedIntegrationEvent).FullName!;

        await channel.ExchangeDeclareAsync(
            exchange: exchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false);

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false);

        await channel.QueueBindAsync(
            queue: queueName,
            exchange: exchangeName,
            routingKey: routingKey);

        var messageId = Guid.NewGuid();

        const string payload =
            """
            {
              "postId": "11111111-1111-1111-1111-111111111111",
              "authorUserId": "22222222-2222-2222-2222-222222222222"
            }
            """;

        await publisher.PublishAsync(
            messageId,
            routingKey,
            payload,
            CancellationToken.None);

        BasicGetResult? result = null;

        for (var i = 0; i < 20 && result is null; i++)
        {
            result = await channel.BasicGetAsync(
                queueName,
                autoAck: true);

            if (result is null)
            {
                await Task.Delay(100);
            }
        }

        Assert.NotNull(result);

        Assert.Equal(
            messageId.ToString(),
            result.BasicProperties.MessageId);

        Assert.Equal(
            routingKey,
            result.BasicProperties.Type);

        Assert.Equal(
            "application/json",
            result.BasicProperties.ContentType);

        var receivedPayload =
            Encoding.UTF8.GetString(
                result.Body.ToArray());

        Assert.Equal(
            payload,
            receivedPayload);
    }
}