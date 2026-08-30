using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Testcontainers.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Template.Api.IntegrationTests.Authentication;
using Template.Modules.Blog.Data;
using Template.Modules.Users.Contracts;
using Template.Modules.Users.Data;
using Template.Modules.Users.Domain;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Template.Api.IntegrationTests.Infrastructure;

public sealed class ApiFactory
    : WebApplicationFactory<Program>,
        IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres =
        new PostgreSqlBuilder("postgres:17")
            .WithDatabase("template_tests")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        using var scope = Services.CreateScope();

        var usersDbContext =
            scope.ServiceProvider
                .GetRequiredService<UsersDbContext>();

        var blogDbContext =
            scope.ServiceProvider
                .GetRequiredService<BlogDbContext>();

        await usersDbContext.Database.MigrateAsync();
        await blogDbContext.Database.MigrateAsync();
    }
    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        await _postgres.DisposeAsync();
    }
    
    public async Task<User> CreateUserAsync(
        string identityId,
        params UserRole[] roles)
    {
        using var scope = Services.CreateScope();

        var dbContext =
            scope.ServiceProvider
                .GetRequiredService<UsersDbContext>();

        var user = new User(
            identityId,
            $"{Guid.NewGuid()}@example.com",
            "Integration Test User");

        foreach (var role in roles)
        {
            user.AddRole(role);
        }

        dbContext.Users.Add(user);

        await dbContext.SaveChangesAsync();

        return user;
    }

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["Authentication:Authority"] =
                        "https://test.invalid",

                    ["Authentication:Audience"] =
                        "template-api",

                    ["ConnectionStrings:UsersDatabase"] =
                        _postgres.GetConnectionString(),

                    ["ConnectionStrings:BlogDatabase"] =
                        _postgres.GetConnectionString()
                });
        });

        builder.ConfigureTestServices(services =>
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme =
                        TestAuthHandler.Scheme;

                    options.DefaultAuthenticateScheme =
                        TestAuthHandler.Scheme;

                    options.DefaultChallengeScheme =
                        TestAuthHandler.Scheme;
                })
                .AddScheme<
                    AuthenticationSchemeOptions,
                    TestAuthHandler>(
                    TestAuthHandler.Scheme,
                    _ => { });
        });
    }
}