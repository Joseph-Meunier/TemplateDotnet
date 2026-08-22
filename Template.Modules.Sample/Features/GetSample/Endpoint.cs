using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;

namespace Template.Modules.Sample.Features.GetSample;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapGetSampleEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/sample", (Handler handler) =>
        {
            var response = handler.Handle();

            return Results.Ok(response);
        });

        return endpoints;
    }
}