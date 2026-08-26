using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Template.Shared.Validation;

namespace Template.Modules.Blog.Features.CreatePost;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapCreatePostEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/blog/posts", async (
                Request request,
                Handler handler,
                CancellationToken cancellationToken) =>
            {
                var response = await handler.Handle(
                    request,
                    cancellationToken);

                return Results.Created(
                    $"/blog/post/{response.Id}",
                    response);
            })
            .AddEndpointFilter<ValidationFilter<Request>>()
            .Produces<Response>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        return endpoints;
    }
}