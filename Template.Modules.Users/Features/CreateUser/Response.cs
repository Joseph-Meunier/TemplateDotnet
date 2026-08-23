namespace Template.Modules.Users.Features.CreateUser;

public sealed record Response(
    Guid Id,
    string Email,
    string DisplayName,
    DateTimeOffset CreatedAt);