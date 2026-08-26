using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Template.Api.Authentication;
using Template.Api.Errors;
using Template.Modules.Users;
using Template.Modules.Blog;
using Template.Modules.Blog.Bootstrap;
using Template.Modules.Users.Bootstrap;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(options =>
{
    options.CreateSchemaReferenceId = type =>
        type.Type.FullName?.Replace("+", ".");

    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new OpenApiComponents();

        document.Components.SecuritySchemes ??=
            new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes["OAuth2"] =
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Description = "Keycloak OAuth2 Authorization Code with PKCE",
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(
                            "https://keycloak-joseph.duckdns.org/realms/template/protocol/openid-connect/auth"),

                        TokenUrl = new Uri(
                            "https://keycloak-joseph.duckdns.org/realms/template/protocol/openid-connect/token"),

                        Scopes = new Dictionary<string, string>
                        {
                            ["openid"] = "OpenID",
                            ["profile"] = "Profile",
                            ["email"] = "Email"
                        }
                    }
                }
            };

        return Task.CompletedTask;
    });
});

builder.Services.AddTemplateAuthentication(
    builder.Configuration);

builder.Services.AddAuthorization();

// Add ProblemDetails middleware for standardized error responses
builder.Services.AddProblemDetails();

// Add global exception handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Add Modules services
builder.Services.AddUsersModule(builder.Configuration);
builder.Services.AddBlogModule(builder.Configuration);

var app = builder.Build();

// Use ProblemDetails middleware to handle exceptions and return standardized error responses
app.UseExceptionHandler();

// Authentication and Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Bootstrap the admin user if we dont have one yet. This is useful for development and testing purposes.
using (var scope = app.Services.CreateScope())
{
    var bootstrapAdmin =
        scope.ServiceProvider
            .GetRequiredService<BootstrapAdminService>();

    await bootstrapAdmin.RunAsync();
}

if (app.Environment.IsDevelopment())
{
    // Seed development data for the blog module
    using var scope = app.Services.CreateScope();

    var blogSeeder =
        scope.ServiceProvider
            .GetRequiredService<DevelopmentBlogSeeder>();

    await blogSeeder.RunAsync();
    
    
    // Enable OpenAPI/Swagger in development environment
    app.MapOpenApi();

    // Document the API with Scalar API Reference
    app.MapScalarApiReference(options =>
    {
        options
            .AddPreferredSecuritySchemes("OAuth2")
            .AddAuthorizationCodeFlow("OAuth2", flow =>
            {
                flow.ClientId = "template-scalar";
                flow.Pkce = Pkce.Sha256;

                flow.SelectedScopes =
                [
                    "openid",
                    "profile",
                    "email"
                ];
            });
    });
}

// Map Modules endpoints
app.MapUsersModule();
app.MapBlogModule();

app.Run();

// Required for integration tests
public partial class Program;