using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Template.Shared.Errors;

namespace Template.Modules.Sample.Features.ErrorSample;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapErrorSampleEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(
            "/sample/errors/not-found",
            () =>
            {
                throw new NotFoundException(
                    "samples.not_found",
                    "The requested sample does not exist.");
            });

        endpoints.MapGet(
            "/sample/errors/conflict",
            () =>
            {
                throw new ConflictException(
                    "samples.already_exists",
                    "The sample already exists.");
            });

        return endpoints;
    }
}