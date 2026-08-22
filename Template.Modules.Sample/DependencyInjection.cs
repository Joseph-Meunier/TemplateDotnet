namespace Template.Modules.Sample;

using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddSampleModule(
        this IServiceCollection services)
    {
        services.AddScoped<Features.GetSample.Handler>();
        services.AddScoped<Features.Echo.Handler>();
            
        return services;
    }
}