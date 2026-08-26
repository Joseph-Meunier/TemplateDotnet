using Template.Modules.Users.Contracts;

namespace Template.Modules.Users.Features.AddUserRole;

public sealed record Request(
    UserRole Role);