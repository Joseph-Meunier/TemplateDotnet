namespace Template.Shared.Events;

public interface IOutboxProcessor
{
    Task ProcessAsync(
        CancellationToken cancellationToken);
}