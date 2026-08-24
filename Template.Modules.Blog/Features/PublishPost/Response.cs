namespace Template.Modules.Blog.Features.PublishPost;

public sealed record Response(
    Guid Id,
    bool IsPublished,
    DateOnly? PublishedAt);