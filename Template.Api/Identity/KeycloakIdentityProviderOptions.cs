using System.ComponentModel.DataAnnotations;

namespace Template.Api.Identity;

public sealed class KeycloakIdentityProviderOptions
{
    public const string SectionName = "Keycloak:CreateUserClient";

    [Required]
    [Url]
    public required string Authority { get; init; }

    [Required]
    public required string Realm { get; init; }

    [Required]
    public required string ClientId { get; init; }

    [Required]
    public required string ClientSecret { get; init; }
}
