using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.Modules.Users.Authorization;
using Template.Modules.Users.Bootstrap;
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
                configuration.GetConnectionString("UsersDatabase"),
                npgsql =>
                {
                    npgsql.MigrationsHistoryTable(
                        "__EFMigrationsHistory",
                        "users");
                });
        });
        
        // Bootstrap admin (if enabled) will run on application startup and assign the Admin role to the user with the specified IdentityId.
        services
            .AddOptions<BootstrapAdminOptions>()
            .Bind(
                configuration.GetSection(
                    BootstrapAdminOptions.SectionName));
        services.AddScoped<BootstrapAdminService>();
        
        services.AddScoped<UsersAuthorizationService>();
        
        services.AddScoped<IUserReader, UserReader>();

        services.AddValidatorsFromAssemblyContaining<ModuleMarker>();

        services.AddScoped<Features.CreateUser.Handler>();
        services.AddScoped<Features.RegisterUser.Handler>();
        services.AddScoped<Features.GetUser.Handler>();
        services.AddScoped<Features.AddUserRole.Handler>();
        services.AddScoped<Features.DeleteUserRole.Handler>();

        return services;
    }
}
