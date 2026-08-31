using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Template.Shared.Validation;

namespace Template.Modules.Users.Features.RegisterUser;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapRegisterUserEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/auth/register", async (
                Request request,
                Handler handler,
                CancellationToken cancellationToken) =>
            {
                var response = await handler.Handle(request, cancellationToken);
                return Results.Created($"/users/{response.Id}", response);
            })
            .AddEndpointFilter<ValidationFilter<Request>>()
            .Produces<Response>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict)
            .AllowAnonymous();

        return endpoints;
    }
}
