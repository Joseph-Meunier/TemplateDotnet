namespace Template.Shared.Events;

public interface IDomainEvent
{
    DateTimeOffset OccurredAt { get; }
}