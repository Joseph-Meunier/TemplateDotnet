using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Template.Shared.Auth;

namespace Template.Api.Authentication;

public static class DependencyInjection
{
    public static IServiceCollection AddTemplateAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        
        services
            .AddOptions<AuthenticationOptions>()
            .Bind(
                configuration.GetSection(
                    AuthenticationOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var authenticationOptions =
            configuration
                .GetSection(AuthenticationOptions.SectionName)
                .Get<AuthenticationOptions>()
            ?? throw new InvalidOperationException(
                "Authentication configuration is missing.");

        services
            .AddAuthentication(
                JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority =
                    authenticationOptions.Authority;

                options.Audience =
                    authenticationOptions.Audience;

                options.RequireHttpsMetadata = true;
            });

        services.AddAuthorization();
        

        return services;
    }
}