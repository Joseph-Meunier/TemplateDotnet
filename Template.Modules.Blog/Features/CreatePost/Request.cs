public sealed record Request(
    Guid AuthorUserId,
    string Title,
    string Description,
    string Content,
    DateOnly StartDate,
    string? HeroImage,
    int ReadingTimeMinutes,
    IReadOnlyCollection<string> Tags);