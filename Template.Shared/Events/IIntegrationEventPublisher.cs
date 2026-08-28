namespace Template.Shared.Events;

public interface IIntegrationEventPublisher
{
    Task PublishAsync(
        Guid messageId,
        string type,
        string payload,
        CancellationToken cancellationToken);
}