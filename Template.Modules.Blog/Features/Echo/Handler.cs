namespace Template.Modules.Sample.Features.Echo;

public sealed class Handler
{
    public async Task<Response> Handle(
        Request request,
        CancellationToken cancellationToken)
    {
        await Task.Delay(100, cancellationToken);

        return new Response(
            request.Message,
            DateTimeOffset.UtcNow);
    }
}