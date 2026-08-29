using Template.Shared.Events;

namespace Template.Api.IntegrationTests.Messaging;

public sealed class TestIntegrationEventPublisher
    : IIntegrationEventPublisher
{
    public List<PublishedMessage> Messages { get; } = [];

    public Exception? ExceptionToThrow { get; set; }

    public Task PublishAsync(
        Guid messageId,
        string type,
        string payload,
        CancellationToken cancellationToken)
    {
        if (ExceptionToThrow is not null)
        {
            throw ExceptionToThrow;
        }

        Messages.Add(
            new PublishedMessage(
                messageId,
                type,
                payload));

        return Task.CompletedTask;
    }
}

public sealed record PublishedMessage(
    Guid MessageId,
    string Type,
    string Payload);