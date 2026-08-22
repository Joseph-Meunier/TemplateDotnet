namespace Template.Modules.Sample.Features.Echo;

public sealed record Response(
    string Message,
    DateTimeOffset ProcessedAt);