namespace Template.Modules.Users.Features.RegisterUser;

public sealed record Response(
    Guid Id,
    string Email,
    string DisplayName,
    DateTimeOffset CreatedAt);
