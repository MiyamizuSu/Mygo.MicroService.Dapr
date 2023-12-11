using System.Reflection;
using Dapr.Client;
using Dapr.Extensions.Configuration;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Identity.Api;
using Infrastructure.Api;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RecAll.Infrastructure.Identity.Api.Data;

namespace RecAll.Infrastructure.Identity.Api;

public static class ProgramExtensions {
    public static void AddCustomConfiguration(
        this WebApplicationBuilder builder) {
        builder.Configuration.AddDaprSecretStore("recall-secretstore",
            new DaprClientBuilder().Build());
    }

    public static void
        AddCustomHealthChecks(this WebApplicationBuilder builder) =>
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy()).AddDapr()
            .AddSqlServer(
                builder.Configuration["ConnectionStrings:IdentityContext"]!,
                name: "TextListDb-check", tags: new[] { "TextListDb" });

    public static void AddCustomIdentityServer(
        this WebApplicationBuilder builder) {
        var connectionString =
            builder.Configuration["ConnectionStrings:IdentityContext"];
        var migrationAssemblyString = typeof(ProgramExtensions).GetTypeInfo()
            .Assembly.GetName().Name;
        builder.Services.AddIdentityServer().AddTestUsers(TestUsers.Users)
            .AddConfigurationStore(o => o.ConfigureDbContext = ob =>
                ob.UseSqlServer(connectionString,
                    sql => sql.MigrationsAssembly(migrationAssemblyString)))
            .AddOperationalStore(o => o.ConfigureDbContext = ob =>
                ob.UseSqlServer(connectionString,
                    sql => sql.MigrationsAssembly(migrationAssemblyString)))
            .AddDeveloperSigningCredential();
    }

    public static async Task ApplyDatabaseMigrationAsync(
        this WebApplication app) {
        using var serviceScope = app.Services.CreateScope();
        var persistedGrantDbContext = serviceScope.ServiceProvider
            .GetRequiredService<PersistedGrantDbContext>();
        var configurationDbContext = serviceScope.ServiceProvider
            .GetRequiredService<ConfigurationDbContext>();
        persistedGrantDbContext.Database.Migrate();
        configurationDbContext.Database.Migrate();
        await new ConfigurationDbContextSeed().SeedAsync(configurationDbContext,
            app.Configuration);
    }
}