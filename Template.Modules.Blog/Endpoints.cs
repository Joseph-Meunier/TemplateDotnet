using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Template.Modules.Sample.Features.CreateSampleItem;
using Template.Modules.Sample.Features.Echo;
using Template.Modules.Sample.Features.ErrorSample;
using Template.Modules.Sample.Features.GetSample;

namespace Template.Modules.Sample;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapSampleModule(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGetSampleEndpoint();
        endpoints.MapEchoEndpoint();
        endpoints.MapCreateSampleItemEndpoint();
        
        endpoints.MapErrorSampleEndpoint();
        
        return endpoints;
    }
}