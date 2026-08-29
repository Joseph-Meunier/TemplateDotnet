namespace Template.Shared.Events;

public sealed record MessageSubscription(
    string QueueName,
    string RoutingKey,
    string DeadLetterQueueName);