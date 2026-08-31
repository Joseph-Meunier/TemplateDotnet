namespace Template.Modules.Users.Features.RegisterUser;

public sealed record Request(
    string Email,
    string DisplayName,
    string Password);
