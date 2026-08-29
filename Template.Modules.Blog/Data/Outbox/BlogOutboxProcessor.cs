using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Template.Shared.Events;

namespace Template.Modules.Blog.Data.Outbox;

internal sealed class BlogOutboxProcessor(
    BlogDbContext dbContext,
    IIntegrationEventPublisher publisher,
    ILogger<BlogOutboxProcessor> logger)
    : IOutboxProcessor
{
    public async Task ProcessAsync(
        CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        const int maxRetryCount = 10;

        var messages = await dbContext.OutboxMessages
            .Where(x =>
                x.ProcessedAt == null &&
                x.FailedAt == null &&
                (x.NextAttemptAt == null ||
                 x.NextAttemptAt <= now))
            .OrderBy(x => x.OccurredAt)
            .Take(20)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                await publisher.PublishAsync(
                    message.Id,
                    message.Type,
                    message.Payload,
                    cancellationToken);
                
                logger.LogInformation(
                    "Outbox message {MessageId} published successfully. Type: {MessageType}",
                    message.Id,
                    message.Type);

                message.MarkAsProcessed(
                    DateTimeOffset.UtcNow);
            }
            catch (Exception exception)
            {
                if (message.RetryCount + 1 >= maxRetryCount)
                {
                    logger.LogError(
                        exception,
                        "Outbox message {MessageId} permanently failed after {RetryCount} attempts",
                        message.Id,
                        message.RetryCount + 1);
                    
                    message.MarkAsFailedPermanently(
                        exception.Message,
                        DateTimeOffset.UtcNow);

                    continue;
                }

                var retryDelay = TimeSpan.FromSeconds(
                    Math.Min(
                        5 * Math.Pow(2, message.RetryCount),
                        300));

                logger.LogWarning(
                    exception,
                    "Outbox message {MessageId} publication failed. Retry {RetryCount} scheduled at {NextAttemptAt}",
                    message.Id,
                    message.RetryCount + 1,
                    DateTimeOffset.UtcNow.Add(retryDelay));
                
                message.MarkAsFailed(
                    exception.Message,
                    DateTimeOffset.UtcNow.Add(retryDelay));
            }
        }

        if (messages.Count > 0)
        {
            await dbContext.SaveChangesAsync(
                cancellationToken);
        }
    }
}