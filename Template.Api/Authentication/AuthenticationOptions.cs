namespace Template.Api.Authentication;

public sealed class AuthenticationOptions
{
    public const string SectionName = "Authentication";

    public required string Authority { get; init; }

    public required string Audience { get; init; }
}