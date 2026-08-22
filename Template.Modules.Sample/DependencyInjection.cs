using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Template.Modules.Sample;

public static class DependencyInjection
{
    public static IServiceCollection AddSampleModule(
        this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<ModuleMarker>();

        services.AddScoped<Features.GetSample.Handler>();
        services.AddScoped<Features.Echo.Handler>();

        return services;
    }
}