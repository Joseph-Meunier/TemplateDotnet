using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Template.Shared.Validation;

namespace Template.Modules.Blog.Features.UpdatePost;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapUpdatePostEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut(
                "/blog/posts/{id:guid}",
                async (
                    Guid id,
                    Request request,
                    Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    var response = await handler.Handle(
                        id,
                        request,
                        cancellationToken);

                    return Results.Ok(response);
                })
            .AddEndpointFilter<ValidationFilter<Request>>()
            .Produces<Response>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);

        return endpoints;
    }
}