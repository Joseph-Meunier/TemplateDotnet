using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Template.Shared.Validation;

namespace Template.Modules.Sample.Features.Echo;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapEchoEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/sample/echo", async (
                Request request,
                Handler handler,
                CancellationToken cancellationToken) =>
            {
                var response = await handler.Handle(
                    request,
                    cancellationToken);

                return Results.Ok(response);
            })
            .AddEndpointFilter<ValidationFilter<Request>>();

        return endpoints;
    }
}