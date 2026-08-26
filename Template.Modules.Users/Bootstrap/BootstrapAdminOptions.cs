namespace Template.Modules.Users.Bootstrap;

public sealed class BootstrapAdminOptions
{
    public const string SectionName = "BootstrapAdmin";

    public bool Enabled { get; init; }

    public string? IdentityId { get; init; }
}