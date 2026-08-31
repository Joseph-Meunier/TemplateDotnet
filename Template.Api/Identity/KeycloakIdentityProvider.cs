using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Template.Modules.Users.Contracts;
using Template.Shared.Errors;

namespace Template.Api.Identity;

public sealed class KeycloakIdentityProvider(
    HttpClient httpClient,
    IOptions<KeycloakIdentityProviderOptions> options)
    : IIdentityProvider
{
    public async Task<string> CreateUserAsync(
        string email,
        string displayName,
        string password,
        CancellationToken cancellationToken)
    {
        var settings = options.Value;
        var token = await GetServiceAccountTokenAsync(settings, cancellationToken);

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"admin/realms/{Uri.EscapeDataString(settings.Realm)}/users")
        {
            Content = JsonContent.Create(new
            {
                username = email,
                email,
                firstName = displayName,
                enabled = true,
                emailVerified = false,
                credentials = new[]
                {
                    new { type = "password", value = password, temporary = false }
                }
            })
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            throw new ConflictException(
                "identity.email_already_exists",
                "An identity already exists with this email.");
        }

        response.EnsureSuccessStatusCode();

        var location = response.Headers.Location
            ?? throw new InvalidOperationException(
                "Keycloak did not return a user location.");

        var identityId = location.Segments[^1].Trim('/');

        if (string.IsNullOrWhiteSpace(identityId))
        {
            throw new InvalidOperationException(
                "Keycloak returned an invalid user location.");
        }

        return identityId;
    }

    private async Task<string> GetServiceAccountTokenAsync(
        KeycloakIdentityProviderOptions settings,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"realms/{Uri.EscapeDataString(settings.Realm)}/protocol/openid-connect/token")
        {
            Content = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", settings.ClientId),
                new KeyValuePair<string, string>("client_secret", settings.ClientSecret)
            ])
        };

        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken);

        return payload?.AccessToken
            ?? throw new InvalidOperationException(
                "Keycloak did not return a service-account access token.");
    }

    private sealed record TokenResponse(
        [property: JsonPropertyName("access_token")]
        string? AccessToken);
}
