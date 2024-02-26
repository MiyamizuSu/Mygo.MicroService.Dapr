using System.IdentityModel.Tokens.Jwt;
using Dapr.Client;
using Dapr.Extensions.Configuration;
using Infrastructure.Api;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Polly;
using RecAll.Contrib.TextList.Api.Filters;
using RecAll.Contrib.TextList.Api.Services;
using Serilog;
using TheSalLab.GeneralReturnValues;

namespace RecAll.Contrib.TextList.Api;

public static class ProgramExtensions {
    public static readonly string AppName = typeof(ProgramExtensions).Namespace;

    public static void AddCustomConfiguration(
        this WebApplicationBuilder builder) {
        builder.Configuration.AddDaprSecretStore("recall-secretstore",
            new DaprClientBuilder().Build());
    }

    public static void AddCustomSerilog(this WebApplicationBuilder builder) {
        var seqServerUrl = builder.Configuration["Serilog:SeqServerUrl"];

        Log.Logger = new LoggerConfiguration().ReadFrom
            .Configuration(builder.Configuration).WriteTo.Console().WriteTo
            .Seq(seqServerUrl).Enrich.WithProperty("ApplicationName", AppName)
            .CreateLogger();

        builder.Host.UseSerilog();
    }

    public static void AddCustomSwagger(this WebApplicationBuilder builder) {
        builder.Services.AddSwaggerGen(options => {
            options.AddSecurityDefinition("oauth2",
                new OpenApiSecurityScheme {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows {
                        Implicit = new OpenApiOAuthFlow {
                            AuthorizationUrl =
                                new Uri(
                                    $"{builder.Configuration["IdentityServer"]}/connect/authorize"),
                            TokenUrl =
                                new Uri(
                                    $"{builder.Configuration["IdentityServer"]}/connect/token"),
                            Scopes = new Dictionary<string, string> {
                                ["TextList"] = "TextList",
                            }
                        }
                    }
                });

            options.OperationFilter<AuthorizeCheckOperationFilter>();
        });
    }

    public static void
        AddCustomHealthChecks(this WebApplicationBuilder builder) =>
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy()).AddDapr()
            .AddSqlServer(
                builder.Configuration["ConnectionStrings:TextListContext"]!,
                name: "TextListDb-check", tags: new[] { "TextListDb" })
            .AddUrlGroup(
                new Uri(builder.Configuration["IdentityServerHealthCheck"]),
                "IdentityServerHealthCheck", tags: new[] { "IdentityServer" });


    public static void UseCustomSwagger(this WebApplication app) {
        app.UseSwagger();
        app.UseSwaggerUI(options => {
            options.OAuthClientId("TextListApiSwaggerUI");
            options.OAuthAppName("TextListApiSwaggerUI");
        });
    }

    public static void AddCustomApplicationServices(
        this WebApplicationBuilder builder) {
        builder.Services.AddScoped<IIdentityService, IdentityService>();
    }

    public static void AddCustomDatabase(this WebApplicationBuilder builder) {
        builder.Services.AddDbContext<TextListContext>(options =>
            options.UseSqlServer(
                builder.Configuration["ConnectionStrings:TextListContext"]));
    }

    public static void AddCustomIdentityService(
        this WebApplicationBuilder builder) {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");
        var identityServerUrl = builder.Configuration["IdentityServer"];
        builder.Services.AddAuthentication(options => {
            options.DefaultAuthenticateScheme =
                JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme =
                JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options => {
            options.Authority = identityServerUrl;
            options.RequireHttpsMetadata = false;
            options.Audience = "TextList";
        });
    }

    public static void ApplyDatabaseMigration(this WebApplication app) {
        // Apply database migration automatically. Note that this approach is not
        // recommended for production scenarios. Consider generating SQL scripts from
        // migrations instead.
        using var scope = app.Services.CreateScope();

        var retryPolicy = CreateRetryPolicy();
        var context =
            scope.ServiceProvider.GetRequiredService<TextListContext>();

        retryPolicy.Execute(context.Database.Migrate);
    }

    private static Policy CreateRetryPolicy() {
        return Policy.Handle<Exception>().WaitAndRetryForever(
            sleepDurationProvider: _ => TimeSpan.FromSeconds(5),
            onRetry: (exception, retry, _) => {
                Console.WriteLine(
                    "Exception {0} with message {1} detected during database migration (retry attempt {2})",
                    exception.GetType().Name, exception.Message, retry);
            });
    }

    public static void AddInvalidModelStateResponseFactory(
        this WebApplicationBuilder builder) {
        builder.Services.AddOptions().Configure<ApiBehaviorOptions>(options => {
            options.InvalidModelStateResponseFactory = context =>
                new OkObjectResult(ServiceResult
                    .CreateInvalidParameterResult(
                        new ValidationProblemDetails(context.ModelState).Errors
                            .Select(p =>
                                $"{p.Key}: {string.Join(" / ", p.Value)}"))
                    .ToServiceResultViewModel());
        });
    }
}