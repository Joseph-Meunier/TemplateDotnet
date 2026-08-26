using System.Net;
using System.Net.Http.Json;
using Template.Api.IntegrationTests.Infrastructure;
using Template.Modules.Users.Contracts;

namespace Template.Api.IntegrationTests.Blog;

public class DeletePostTests(
    ApiFactory factory)
    : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client =
        factory.CreateClient();
   

    // - Creator propriétaire -> 204
    [Fact]
    public async Task DeletePost_WithAuthenticatedUserWithCreatorRoleAndIsOwner_ReturnsNoContent()
    {
        var identityId = $"creator-{Guid.NewGuid()}";

        await factory.CreateUserAsync(
            identityId: identityId,
            roles: [UserRole.Creator]);

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/blog/posts")
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

        // Delete the post as the owner
        using var deleteRequest = new HttpRequestMessage(
            HttpMethod.Delete,
            $"/blog/posts/{createdPost.Id}");

        deleteRequest.Headers.Add(
            "X-Test-Identity",
            identityId);

        var deleteResponse =
            await _client.SendAsync(deleteRequest);

        Assert.Equal(
            HttpStatusCode.NoContent,
            deleteResponse.StatusCode);
    }
        
    // - Creator non propriétaire -> 403 
    [Fact]
    public async Task DeletePost_WithAuthenticatedUserWithCreatorRoleAndIsNotOwner_ReturnsForbidden()
    {
        var identityIdOwner = $"creator-{Guid.NewGuid()}";
        var identityIdNotOwner = $"creator-{Guid.NewGuid()}";

        await factory.CreateUserAsync(
            identityId: identityIdOwner,
            roles: [UserRole.Creator]);

        await factory.CreateUserAsync(
            identityId: identityIdNotOwner,
            roles: [UserRole.Creator]);

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/blog/posts")
        {
            Content = JsonContent.Create(
                CreateValidRequest())
        };

        request.Headers.Add(
            "X-Test-Identity",
            identityIdOwner);

        var response =
            await _client.SendAsync(request);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var createdPost =
            await response.Content
                .ReadFromJsonAsync<CreatePostResponse>();

        // Delete the post as the owner
        using var deleteRequest = new HttpRequestMessage(
            HttpMethod.Delete,
            $"/blog/posts/{createdPost.Id}");

        deleteRequest.Headers.Add(
            "X-Test-Identity",
            identityIdNotOwner);

        var deleteResponse =
            await _client.SendAsync(deleteRequest);

        Assert.Equal(
            HttpStatusCode.Forbidden,
            deleteResponse.StatusCode);
    }

    // - Admin -> 204
    [Fact]
    public async Task DeletePost_WithAuthenticatedUserWithAdminRole_ReturnsNoContent()
    {
        var identityIdCreator = $"creator-{Guid.NewGuid()}";
        var identityIdAdmin = $"creator-{Guid.NewGuid()}";

        await factory.CreateUserAsync(
            identityId: identityIdCreator,
            roles: [UserRole.Creator]);

        await factory.CreateUserAsync(
            identityId: identityIdAdmin,
            roles: [UserRole.Admin]);

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/blog/posts")
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

        // Delete the post as the owner
        using var deleteRequest = new HttpRequestMessage(
            HttpMethod.Delete,
            $"/blog/posts/{createdPost.Id}");

        deleteRequest.Headers.Add(
            "X-Test-Identity",
            identityIdAdmin);

        var deleteResponse =
            await _client.SendAsync(deleteRequest);

        Assert.Equal(
            HttpStatusCode.NoContent,
            deleteResponse.StatusCode);
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