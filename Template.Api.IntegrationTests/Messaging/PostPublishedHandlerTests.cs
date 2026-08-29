using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Template.Api.IntegrationTests.Infrastructure;
using Template.Modules.Blog.Contracts.Events;
using Template.Modules.Blog.Data;
using Template.Shared.Events;

namespace Template.Api.IntegrationTests.Messaging;

public sealed class PostPublishedHandlerTests(
    ApiFactory factory)
    : IClassFixture<ApiFactory>
{
    [Fact]
    public async Task HandleAsync_FirstProcessing_AddsInboxMessage()
    {
        var messageId = Guid.NewGuid();

        var integrationEvent =
            new PostPublishedIntegrationEvent(
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTimeOffset.UtcNow);

        var payload =
            JsonSerializer.Serialize(integrationEvent);

        await using (var scope =
                     factory.Services.CreateAsyncScope())
        {
            var handler =
                scope.ServiceProvider
                    .GetServices<IIntegrationEventHandler>()
                    .Single(x =>
                        x.EventType ==
                        typeof(PostPublishedIntegrationEvent)
                            .FullName);

            await handler.HandleAsync(
                messageId,
                payload,
                CancellationToken.None);
        }

        await using var verificationScope =
            factory.Services.CreateAsyncScope();

        var dbContext =
            verificationScope.ServiceProvider
                .GetRequiredService<BlogDbContext>();

        var inboxMessage =
            await dbContext.InboxMessages
                .SingleAsync(
                    x => x.MessageId == messageId);

        Assert.Equal(
            messageId,
            inboxMessage.MessageId);
    }

    [Fact]
    public async Task HandleAsync_SameMessageTwice_IsIdempotent()
    {
        var messageId = Guid.NewGuid();

        var integrationEvent =
            new PostPublishedIntegrationEvent(
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTimeOffset.UtcNow);

        var payload =
            JsonSerializer.Serialize(integrationEvent);

        for (var i = 0; i < 2; i++)
        {
            await using var scope =
                factory.Services.CreateAsyncScope();

            var handler =
                scope.ServiceProvider
                    .GetServices<IIntegrationEventHandler>()
                    .Single(x =>
                        x.EventType ==
                        typeof(PostPublishedIntegrationEvent)
                            .FullName);

            await handler.HandleAsync(
                messageId,
                payload,
                CancellationToken.None);
        }

        await using var verificationScope =
            factory.Services.CreateAsyncScope();

        var dbContext =
            verificationScope.ServiceProvider
                .GetRequiredService<BlogDbContext>();

        var count =
            await dbContext.InboxMessages
                .CountAsync(
                    x => x.MessageId == messageId);

        Assert.Equal(
            1,
            count);
    }

    [Fact]
    public async Task HandleAsync_InvalidPayload_DoesNotAddInboxMessage()
    {
        var messageId = Guid.NewGuid();

        await using var scope =
            factory.Services.CreateAsyncScope();

        var handler =
            scope.ServiceProvider
                .GetServices<IIntegrationEventHandler>()
                .Single(x =>
                    x.EventType ==
                    typeof(PostPublishedIntegrationEvent)
                        .FullName);

        await Assert.ThrowsAnyAsync<Exception>(
            () => handler.HandleAsync(
                messageId,
                "{ invalid json",
                CancellationToken.None));

        var dbContext =
            scope.ServiceProvider
                .GetRequiredService<BlogDbContext>();

        var exists =
            await dbContext.InboxMessages
                .AnyAsync(
                    x => x.MessageId == messageId);

        Assert.False(exists);
    }
}