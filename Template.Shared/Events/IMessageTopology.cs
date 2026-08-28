namespace Template.Shared.Events;

public interface IMessageTopology
{
    IReadOnlyCollection<MessageSubscription> Subscriptions { get; }
}