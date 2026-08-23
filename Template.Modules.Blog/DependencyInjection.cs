using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.Modules.Sample.Data;

namespace Template.Modules.Sample;

public static class DependencyInjection
{
    public static IServiceCollection AddSampleModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<SampleDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("SampleDatabase"));
        });

        
        services.AddValidatorsFromAssemblyContaining<ModuleMarker>();

        services.AddScoped<Features.GetSample.Handler>();
        services.AddScoped<Features.Echo.Handler>();
        services.AddScoped<Features.CreateSampleItem.Handler>();

        return services;
    }
}