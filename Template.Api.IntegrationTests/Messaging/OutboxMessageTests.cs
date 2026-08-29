using Template.Modules.Blog.Data.Outbox;

namespace Template.Api.IntegrationTests.Messaging;

public sealed class OutboxMessageTests
{
    [Fact]
    public void MarkAsFailed_IncrementsRetryCount()
    {
        var message = CreateMessage();

        var nextAttemptAt =
            DateTimeOffset.UtcNow.AddSeconds(5);

        message.MarkAsFailed(
            "Failure",
            nextAttemptAt);

        Assert.Equal(
            1,
            message.RetryCount);

        Assert.Equal(
            "Failure",
            message.LastError);

        Assert.Equal(
            nextAttemptAt,
            message.NextAttemptAt);

        Assert.Null(
            message.ProcessedAt);

        Assert.Null(
            message.FailedAt);
    }

    [Fact]
    public void MarkAsProcessed_ClearsFailureState()
    {
        var message = CreateMessage();

        message.MarkAsFailed(
            "Failure",
            DateTimeOffset.UtcNow.AddSeconds(5));

        var processedAt =
            DateTimeOffset.UtcNow;

        message.MarkAsProcessed(
            processedAt);

        Assert.Equal(
            processedAt,
            message.ProcessedAt);

        Assert.Null(
            message.LastError);

        Assert.Null(
            message.NextAttemptAt);

        Assert.Null(
            message.FailedAt);
    }

    [Fact]
    public void MarkAsFailedPermanently_SetsFailedAt()
    {
        var message = CreateMessage();

        var failedAt =
            DateTimeOffset.UtcNow;

        message.MarkAsFailedPermanently(
            "Permanent failure",
            failedAt);

        Assert.Equal(
            1,
            message.RetryCount);

        Assert.Equal(
            "Permanent failure",
            message.LastError);

        Assert.Equal(
            failedAt,
            message.FailedAt);

        Assert.Null(
            message.NextAttemptAt);

        Assert.Null(
            message.ProcessedAt);
    }

    private static OutboxMessage CreateMessage()
    {
        return new OutboxMessage(
            Guid.NewGuid(),
            "TestEvent",
            "{}",
            DateTimeOffset.UtcNow);
    }
}