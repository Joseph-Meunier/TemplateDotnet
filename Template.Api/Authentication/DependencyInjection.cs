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

        services
            .AddAuthentication(
                JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services
            .AddOptions<JwtBearerOptions>(
                JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<AuthenticationOptions>>(
                (jwtOptions, authenticationOptions) =>
                {
                    var settings =
                        authenticationOptions.Value;

                    jwtOptions.Authority =
                        settings.Authority;

                    jwtOptions.Audience =
                        settings.Audience;

                    jwtOptions.RequireHttpsMetadata = true;
                    jwtOptions.MapInboundClaims = false;
                });

        services.AddAuthorization();

        return services;
    }
}