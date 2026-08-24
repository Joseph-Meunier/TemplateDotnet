namespace Template.Modules.Blog.Features.GetPost;

public sealed record Response(
    Guid Id,
    Guid AuthorUserId,
    string Title,
    string Description,
    string Content,
    DateOnly StartDate,
    DateOnly? PublishedAt,
    DateTimeOffset UpdatedAt,
    string? HeroImage,
    bool IsPublished,
    int ReadingTimeMinutes,
    IReadOnlyCollection<string> Tags);