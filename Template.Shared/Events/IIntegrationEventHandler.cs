namespace Template.Shared.Events;

public interface IIntegrationEventHandler
{
    string EventType { get; }

    Task HandleAsync(
        Guid messageId,
        string payload,
        CancellationToken cancellationToken);
}