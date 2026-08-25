using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Testcontainers.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Template.Modules.Blog.Data;
using Template.Modules.Users.Data;

namespace Template.Api.IntegrationTests.Infrastructure;

public sealed class ApiFactory
    : WebApplicationFactory<Program>,
        IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres =
        new PostgreSqlBuilder()
            .WithImage("postgres:17")
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
        await _postgres.DisposeAsync();
    }

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(
            (_, configuration) =>
            {
                var connectionString =
                    _postgres.GetConnectionString();

                configuration.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:UsersDatabase"] =
                            connectionString,

                        ["ConnectionStrings:BlogDatabase"] =
                            connectionString
                    });
            });
    }
}