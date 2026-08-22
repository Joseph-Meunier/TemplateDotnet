using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Template.Api.IntegrationTests.Infrastructure;

namespace Template.Api.IntegrationTests.Sample;

public sealed class EchoTests
    : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public EchoTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Echo_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new
        {
            message = "Hello"
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/sample/echo",
            request);

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }
    
    [Fact]
    public async Task Echo_WithValidRequest_ReturnsExpectedResponse()
    {
        // Arrange
        var request = new
        {
            message = "Hello"
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/sample/echo",
            request);

        var content =
            await response.Content.ReadFromJsonAsync<EchoResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.NotNull(content);
        Assert.Equal("Hello", content.Message);
    }
    
    [Fact]
    public async Task Echo_WithEmptyMessage_ReturnsBadRequest()
    {
        // Arrange
        var request = new
        {
            message = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/sample/echo",
            request);

        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }
    
    [Fact]
    public async Task Echo_WithEmptyMessage_ReturnsValidationProblem()
    {
        var request = new
        {
            message = ""
        };

        var response = await _client.PostAsJsonAsync(
            "/sample/echo",
            request);

        var problem =
            await response.Content
                .ReadFromJsonAsync<HttpValidationProblemDetails>();

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);

        Assert.NotNull(problem);

        Assert.True(
            problem.Errors.ContainsKey("Message"));
    }
}