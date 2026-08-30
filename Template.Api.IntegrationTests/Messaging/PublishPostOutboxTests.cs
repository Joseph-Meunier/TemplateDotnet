using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Template.Api.IntegrationTests.Infrastructure;
using Template.Modules.Blog.Contracts.Events;
using Template.Modules.Blog.Data;
using Template.Modules.Blog.Domain;
using Template.Modules.Users.Contracts;

namespace Template.Api.IntegrationTests.Messaging;

public sealed class PublishPostOutboxTests(
    ApiFactory factory)
    : IClassFixture<ApiFactory>
{
    [Fact]
    public async Task PublishPost_CreatesOutboxMessage()
    {
        const string identityId = "outbox-creator";

        var user = await factory.CreateUserAsync(
            identityId,
            UserRole.Creator);

        Guid postId;

        await using (var scope =
                     factory.Services.CreateAsyncScope())
        {
            var dbContext =
                scope.ServiceProvider
                    .GetRequiredService<BlogDbContext>();

            var post = new Post(
                user.Id,
                "Outbox test",
                "Description",
                "Content",
                DateOnly.FromDateTime(DateTime.UtcNow),
                null,
                5);

            dbContext.Posts.Add(post);

            await dbContext.SaveChangesAsync();

            postId = post.Id;
        }

        var client = factory.CreateClient();

        client.DefaultRequestHeaders.Add(
            "X-Test-Identity",
            identityId);

        var response = await client.PostAsync(
            $"/blog/posts/{postId}/publish",
            null);

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        await using var verificationScope =
            factory.Services.CreateAsyncScope();

        var verificationDbContext =
            verificationScope.ServiceProvider
                .GetRequiredService<BlogDbContext>();

        var outboxMessage =
            await verificationDbContext.OutboxMessages
                .SingleAsync();

        Assert.Equal(
            typeof(PostPublishedIntegrationEvent).FullName,
            outboxMessage.Type);

        Assert.Null(outboxMessage.ProcessedAt);
        Assert.Null(outboxMessage.FailedAt);
        Assert.Null(outboxMessage.LastError);
        Assert.Null(outboxMessage.NextAttemptAt);
        Assert.Equal(0, outboxMessage.RetryCount);

        Assert.Contains(
            postId.ToString(),
            outboxMessage.Payload);
    }
}