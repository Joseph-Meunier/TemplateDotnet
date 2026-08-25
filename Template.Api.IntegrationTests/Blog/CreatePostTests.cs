using System.Net;
using System.Net.Http.Json;
using Template.Api.IntegrationTests.Infrastructure;

namespace Template.Api.IntegrationTests.Blog;

public sealed class CreatePostTests(
    ApiFactory factory)
    : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client =
        factory.CreateClient();

    private sealed record UserResponse(
        Guid Id,
        string Email,
        string DisplayName,
        DateTimeOffset CreatedAt);

    [Fact]
    public async Task CreatePost_WithExistingAuthor_ReturnsCreated()
    {
        var userRequest = new
        {
            email = $"author-{Guid.NewGuid()}@example.com",
            displayName = "Author"
        };

        var userHttpResponse =
            await _client.PostAsJsonAsync(
                "/users",
                userRequest);

        userHttpResponse.EnsureSuccessStatusCode();

        var user =
            await userHttpResponse.Content
                .ReadFromJsonAsync<UserResponse>();

        Assert.NotNull(user);

        var postRequest = new
        {
            authorUserId = user.Id,
            title = "Test post",
            description = "Test description",
            content = "# Test",
            startDate = "2026-08-24",
            heroImage = (string?)null,
            readingTimeMinutes = 5,
            tags = new[]
            {
                "dotnet",
                "testcontainers"
            }
        };

        var response =
            await _client.PostAsJsonAsync(
                "/blog/post",
                postRequest);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);
    }
    
    [Fact]
    public async Task CreatePost_WithUnknownAuthor_ReturnsNotFound()
    {
        var request = new
        {
            authorUserId = Guid.NewGuid(),
            title = "Test post",
            description = "Test description",
            content = "# Test",
            startDate = "2026-08-24",
            heroImage = (string?)null,
            readingTimeMinutes = 5,
            tags = new[]
            {
                "dotnet"
            }
        };

        var response =
            await _client.PostAsJsonAsync(
                "/blog/post",
                request);

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }
}