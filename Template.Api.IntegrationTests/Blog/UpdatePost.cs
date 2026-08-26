using System.Net;
using System.Net.Http.Json;
using Template.Api.IntegrationTests.Infrastructure;
using Template.Modules.Blog.Domain;
using Template.Modules.Users.Contracts;

namespace Template.Api.IntegrationTests.Blog;

public sealed class UpdatePostTests(
    ApiFactory factory)
    : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client =
        factory.CreateClient();

    [Fact]
    public async Task UpdatePost_WithAuthenticatedUserWithCreatorRoleAndIsOwner_ReturnsOk()
    {
        var identityId = $"creator-{Guid.NewGuid()}";

        await factory.CreateUserAsync(
            identityId: identityId,
            roles: [UserRole.Creator]);

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/blog/post")
        {
            Content = JsonContent.Create(
                CreateValidRequest())
        };

        request.Headers.Add(
            "X-Test-Identity",
            identityId);

        var response =
            await _client.SendAsync(request);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);
        
        var createdPost =
            await response.Content
                .ReadFromJsonAsync<CreatePostResponse>();

        // Update the post as the owner
        using var updateRequest = new HttpRequestMessage(
            HttpMethod.Put,
            $"/blog/post/{createdPost.Id}")
        {
            Content = JsonContent.Create(
                UpdateValidRequest()
            )
        };

        updateRequest.Headers.Add(
            "X-Test-Identity",
            identityId);

        var updateResponse =
            await _client.SendAsync(updateRequest);

        var body = await updateResponse.Content.ReadAsStringAsync();

        Assert.Equal(
            HttpStatusCode.OK,
            updateResponse.StatusCode);
    }
    
    [Fact]
    public async Task UpdatePost_WithAuthenticatedUserWithCreatorRoleAndIsNotOwner_ReturnsForbidden()
    {
        var identityIdTrueOwner = $"creator-{Guid.NewGuid()}";
        var identityIdFalseOwner = $"creator-{Guid.NewGuid()}";

        await factory.CreateUserAsync(
            identityId: identityIdTrueOwner,
            roles: [UserRole.Creator]);
        
        await factory.CreateUserAsync(
            identityId: identityIdFalseOwner,
            roles: [UserRole.Creator]);

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/blog/post")
        {
            Content = JsonContent.Create(
                CreateValidRequest())
        };

        request.Headers.Add(
            "X-Test-Identity",
            identityIdTrueOwner);

        var response =
            await _client.SendAsync(request);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);
        
        var createdPost =
            await response.Content
                .ReadFromJsonAsync<CreatePostResponse>();

        // Update the post as the owner
        using var updateRequest = new HttpRequestMessage(
            HttpMethod.Put,
            $"/blog/post/{createdPost.Id}")
        {
            Content = JsonContent.Create(
                UpdateValidRequest()
            )
        };

        updateRequest.Headers.Add(
            "X-Test-Identity",
            identityIdFalseOwner);

        var updateResponse =
            await _client.SendAsync(updateRequest);

        var body = await updateResponse.Content.ReadAsStringAsync();

        Assert.Equal(
            HttpStatusCode.Forbidden,
            updateResponse.StatusCode);
    }
    
    [Fact]
    public async Task UpdatePost_WithAuthenticatedUserWithAdminRole_ReturnsForbiden()
    {
        var identityIdCreator = $"creator-{Guid.NewGuid()}";
        var identityIdAdmin = $"admin-{Guid.NewGuid()}";

        await factory.CreateUserAsync(
            identityId: identityIdCreator,
            roles: [UserRole.Creator]);

        await factory.CreateUserAsync(
            identityId: identityIdAdmin,
            roles: [UserRole.Admin]);

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/blog/post")
        {
            Content = JsonContent.Create(
                CreateValidRequest())
        };

        request.Headers.Add(
            "X-Test-Identity",
            identityIdCreator);

        var response =
            await _client.SendAsync(request);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);
        
        var createdPost =
            await response.Content
                .ReadFromJsonAsync<CreatePostResponse>();

        // Update the post as the owner
        using var updateRequest = new HttpRequestMessage(
            HttpMethod.Put,
            $"/blog/post/{createdPost.Id}")
        {
            Content = JsonContent.Create(
                UpdateValidRequest()
            )
        };

        updateRequest.Headers.Add(
            "X-Test-Identity",
            identityIdAdmin);

        var updateResponse =
            await _client.SendAsync(updateRequest);

        var body = await updateResponse.Content.ReadAsStringAsync();

        Assert.Equal(
            HttpStatusCode.Forbidden,
            updateResponse.StatusCode);
    }
    
    private sealed record CreatePostResponse(
    Guid Id,
    string Title);

    private static object StartTags()
    {
        return new[]
        {
            "dotnet",
            "testcontainers"
        };
    }
    
    private static object NewTags()
    {
        return new[]
        {
            "dotnet",
            "testcontainers",
            "updated"
        };
    }
    
    private static object CreateValidRequest()
    {
        return new
        {
            title = "Test post",
            description = "Test description",
            content = "# Test",
            startDate = "2026-08-24",
            heroImage = (string?)null,
            readingTimeMinutes = 5,
            tags = StartTags()
        };
    }

    private static object UpdateValidRequest()
    {
        return new
        {
            title = "Updated Test post",
            description = "Updated Test description",
            content = "# Updated Test",
            startDate = "2026-08-25",
            heroImage = (string?)null,
            readingTimeMinutes = 10,
            tags = NewTags()
        };
    }
}