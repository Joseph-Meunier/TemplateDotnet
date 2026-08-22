namespace Template.Api.IntegrationTests.Sample;

public sealed record EchoResponse(
    string Message,
    DateTimeOffset ProcessedAt);