namespace Template.Modules.Users.Features.CreateUser;

public sealed record Request(
    string Email,
    string DisplayName);