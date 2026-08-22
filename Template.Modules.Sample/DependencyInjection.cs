namespace Template.Modules.Sample;

using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddSampleModule(
        this IServiceCollection services)
    {
        return services;
    }
}