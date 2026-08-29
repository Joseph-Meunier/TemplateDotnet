namespace Template.Modules.Blog.Data.Outbox;

public sealed class OutboxMessage
{
    public Guid Id { get; private set; }

    public string Type { get; private set; } = null!;

    public string Payload { get; private set; } = null!;

    public DateTimeOffset OccurredAt { get; private set; }

    public DateTimeOffset? ProcessedAt { get; private set; }
    
    public int RetryCount { get; private set; }

    public string? LastError { get; private set; }

    public DateTimeOffset? NextAttemptAt { get; private set; }
    
    public DateTimeOffset? FailedAt { get; private set; }
    
    public void MarkAsFailed(
        string error,
        DateTimeOffset nextAttemptAt)
    {
        RetryCount++;
        LastError = error;
        NextAttemptAt = nextAttemptAt;
    }
    
    public void MarkAsFailedPermanently(
        string error,
        DateTimeOffset failedAt)
    {
        RetryCount++;
        LastError = error;
        NextAttemptAt = null;
        FailedAt = failedAt;
    }
    
    public void MarkAsProcessed(DateTimeOffset processedAt)
    {
        ProcessedAt = processedAt;
        LastError = null;
        NextAttemptAt = null;
        FailedAt = null;
    }

    private OutboxMessage()
    {
    }

    public OutboxMessage(
        Guid id,
        string type,
        string payload,
        DateTimeOffset occurredAt)
    {
        Id = id;
        Type = type;
        Payload = payload;
        OccurredAt = occurredAt;
    }
}