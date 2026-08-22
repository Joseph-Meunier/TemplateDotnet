using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc;

namespace Template.Api.IntegrationTests.Sample;

public sealed class ErrorHandlingTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ErrorHandlingTests(
        WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task NotFoundException_Returns404ProblemDetails()
    {
        // Act
        var response = await _client.GetAsync(
            "/sample/errors/not-found");

        var problem =
            await response.Content
                .ReadFromJsonAsync<ProblemDetails>();

        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);

        Assert.NotNull(problem);

        Assert.Equal(
            "Resource not found",
            problem.Title);
    }
    
    [Fact]
    public async Task NotFoundException_ReturnsExpectedErrorCode()
    {
        var response = await _client.GetAsync(
            "/sample/errors/not-found");

        var problem =
            await response.Content
                .ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);

        Assert.NotNull(problem);

        var code =
            Assert.IsType<JsonElement>(
                problem.Extensions["code"]);

        Assert.Equal(
            "samples.not_found",
            code.GetString());
    }
    
    [Fact]
    public async Task ConflictException_Returns409()
    {
        var response = await _client.GetAsync(
            "/sample/errors/conflict");

        var problem =
            await response.Content
                .ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(
            HttpStatusCode.Conflict,
            response.StatusCode);

        Assert.NotNull(problem);

        var code =
            Assert.IsType<JsonElement>(
                problem.Extensions["code"]);

        Assert.Equal(
            "samples.already_exists",
            code.GetString());
    }
}