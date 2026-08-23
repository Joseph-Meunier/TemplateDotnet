using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.Modules.Blog.Data;

namespace Template.Modules.Blog;

public static class DependencyInjection
{
    public static IServiceCollection AddBlogModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<BlogDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("BlogDatabase"));
        });

        services.AddValidatorsFromAssemblyContaining<ModuleMarker>();

        services.AddScoped<Features.CreatePost.Handler>();

        return services;
    }
}