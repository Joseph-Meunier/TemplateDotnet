using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Template.Shared.Validation;

namespace Template.Modules.Sample.Features.CreateSampleItem;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapCreateSampleItemEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/sample/items", async (
                Request request,
                Handler handler,
                CancellationToken cancellationToken) =>
            {
                var response = await handler.Handle(
                    request,
                    cancellationToken);

                return Results.Created(
                    $"/sample/items/{response.Id}",
                    response);
            })
            .AddEndpointFilter<ValidationFilter<Request>>()
            .Produces<Response>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        return endpoints;
    }
}