using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.Modules.Users.Contracts;
using Template.Modules.Users.Data;

namespace Template.Modules.Users;

public static class DependencyInjection
{
    public static IServiceCollection AddUsersModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<UsersDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("UsersDatabase"));
        });
        
        services.AddScoped<IUserReader, UserReader>();

        services.AddValidatorsFromAssemblyContaining<ModuleMarker>();

        services.AddScoped<Features.CreateUser.Handler>();
        services.AddScoped<Features.GetUser.Handler>();

        return services;
    }
}