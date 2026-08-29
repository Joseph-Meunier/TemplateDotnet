namespace Template.Modules.Blog.Data.Inbox;

public sealed class InboxMessage
{
    public Guid MessageId { get; private set; }

    public DateTimeOffset ProcessedAt { get; private set; }

    private InboxMessage()
    {
    }

    public InboxMessage(
        Guid messageId,
        DateTimeOffset processedAt)
    {
        MessageId = messageId;
        ProcessedAt = processedAt;
    }
}