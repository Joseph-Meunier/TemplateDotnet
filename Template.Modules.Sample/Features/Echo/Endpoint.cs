using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Template.Modules.Sample.Features.Echo;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapEchoEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/sample/echo", async (
            Request request,
            IValidator<Request> validator,
            Handler handler,
            CancellationToken cancellationToken) =>
        {
            var validationResult = await validator.ValidateAsync(
                request,
                cancellationToken);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(
                    validationResult.ToDictionary());
            }

            var response = await handler.Handle(
                request,
                cancellationToken);

            return Results.Ok(response);
        });

        return endpoints;
    }
}