namespace Template.Modules.Blog.Features.GetPublishedPosts;

public sealed record PostItem(
    Guid Id,
    string Title,
    string Description,
    DateOnly? PublishedAt,
    string? HeroImage,
    int ReadingTimeMinutes,
    IReadOnlyCollection<string> Tags);

public sealed record Response(
    IReadOnlyCollection<PostItem> Items,
    int Page,
    int PageSize,
    int TotalCount);