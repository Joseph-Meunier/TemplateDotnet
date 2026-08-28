namespace Template.Modules.Blog.IntegrationEvents;

public sealed record PostPublishedIntegrationEvent(
    Guid PostId,
    Guid AuthorUserId,
    DateTimeOffset PublishedAt);