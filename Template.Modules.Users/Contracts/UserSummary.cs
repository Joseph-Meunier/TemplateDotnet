using Template.Modules.Users.Contracts;

public sealed record UserSummary(
    Guid Id,
    string Email,
    string DisplayName,
    IReadOnlyCollection<UserRole> Roles);