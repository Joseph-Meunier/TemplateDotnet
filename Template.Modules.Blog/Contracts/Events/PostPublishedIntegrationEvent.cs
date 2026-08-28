namespace Template.Modules.Blog.Contracts.Events;

public sealed record PostPublishedIntegrationEvent(
    Guid PostId,
    Guid AuthorUserId,
    DateTimeOffset PublishedAt);