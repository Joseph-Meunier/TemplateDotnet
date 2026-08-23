namespace Template.Modules.Users.Features.GetUser;

public sealed record Response(
    Guid Id,
    string Email,
    string DisplayName,
    DateTimeOffset CreatedAt);