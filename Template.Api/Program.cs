using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Template.Api.Authentication;
using Template.Api.Infrastructure.Health;
using Template.Api.Infrastructure.Messaging;
using Template.Modules.Users;
using Template.Modules.Blog;
using Template.Modules.Blog.Bootstrap;
using Template.Modules.Blog.Data;
using Template.Modules.Users.Bootstrap;
using Template.Modules.Users.Data;
using Template.Shared.Events;

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


// Add rabbitmq and outbox services
builder.Services.Configure<RabbitMqOptions>(
    builder.Configuration.GetSection(
        RabbitMqOptions.SectionName));

builder.Services.AddSingleton<RabbitMqConnection>();

builder.Services.AddSingleton<IIntegrationEventPublisher,
    RabbitMqIntegrationEventPublisher>();

builder.Services.AddSingleton<RabbitMqTopologyInitializer>();

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddHostedService<OutboxWorker>();
    builder.Services.AddHostedService<RabbitMqConsumerWorker>();
}

// Add ProblemDetails middleware for standardized error responses
builder.Services.AddProblemDetails();

// Add global exception handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Add Modules services
builder.Services.AddUsersModule(builder.Configuration);
builder.Services.AddBlogModule(builder.Configuration);

builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<UsersDbContext>(
        name: "users-db",
        tags: ["ready"])
    .AddDbContextCheck<BlogDbContext>(
        name: "blog-db",
        tags: ["ready"])
    .AddCheck<RabbitMqHealthCheck>(
        name: "rabbitmq",
        tags: ["ready"]);

var app = builder.Build();

// Initialize the RabbitMQ topology
if (!app.Environment.IsEnvironment("Testing"))
{
    await using var scope = app.Services.CreateAsyncScope();

    var topologyInitializer =
        scope.ServiceProvider
            .GetRequiredService<RabbitMqTopologyInitializer>();

    await topologyInitializer.InitializeAsync(
        CancellationToken.None);
}

// Use ProblemDetails middleware to handle exceptions and return standardized error responses
app.UseExceptionHandler();

// Authentication and Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Bootstrap the admin user if we don't have one yet. This is useful for development and testing purposes.
if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();

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

app.MapHealthChecks(
    "/health/live",
    new HealthCheckOptions
    {
        Predicate = _ => false
    });


app.MapHealthChecks(
    "/health/ready",
    new HealthCheckOptions
    {
        Predicate = check =>
            check.Tags.Contains("ready"),

        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = report.Status.ToString(),

                checks = report.Entries.Select(entry => new
                {
                    name = entry.Key,
                    status = entry.Value.Status.ToString(),
                    description = entry.Value.Description
                })
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    });

app.Run();

// Required for integration tests
public partial class Program;