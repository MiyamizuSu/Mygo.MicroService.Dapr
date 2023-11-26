// var builder = WebApplication.CreateBuilder(args);
// var app = builder.Build();
//
// app.MapGet("/", () => "Hello World!");
//
// app.Run();

using System.Reflection;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RecAll.Core.List.Api;
using RecAll.Core.List.Api.Infrastructure;
using RecAll.Core.List.Api.Infrastructure.Filters;
using RecAll.Core.List.Api.Infrastructure.Services;
using RecAll.Core.List.Infrastructure;
using Serilog;
using TheSalLab.GeneralReturnValues;

var builder = WebApplication.CreateBuilder(args);

builder.AddCustomConfiguration();
builder.AddCustomServiceProviderFactory();
builder.AddCustomSerilog();
builder.AddCustomSwagger();

// TODO

builder.Services.AddDbContext<ListContext>(options => {
    options.UseSqlServer(builder.Configuration["ListContext"],
        sqlServerOptionsAction => {
            sqlServerOptionsAction.MigrationsAssembly(typeof(InitialFunctions)
                .GetTypeInfo().Assembly.GetName().Name);
            sqlServerOptionsAction.EnableRetryOnFailure(15,
                TimeSpan.FromSeconds(30), null);
        });
});

builder.Services.AddTransient<IIdentityService, MockIdentityService>();

builder.Services.AddCors(options => {
    options.AddPolicy("CorsPolicy",
        builder => builder.SetIsOriginAllowed(host => true).AllowAnyMethod()
            .AllowAnyHeader().AllowCredentials());
});

builder.Services
    .AddControllers(options =>
        options.Filters.Add(typeof(HttpGlobalExceptionFilter)))
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.IncludeFields = true);

builder.Services.AddOptions().Configure<ApiBehaviorOptions>(options => {
    options.InvalidModelStateResponseFactory = context =>
        new OkObjectResult(ServiceResult.CreateInvalidParameterResult(
                new ValidationProblemDetails(context.ModelState).Errors.Select(
                    p =>
                        $"{p.Key}: {string.Join(" / ", p.Value)}"))
            .ToServiceResultViewModel());
});

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy()).AddUrlGroup(
        new Uri(builder.Configuration["TextListHealthCheck"]),
        "TextListHealthCheck", tags: new[] { "TextList" });

var app = builder.Build();

Log.Information("Configuring web host ({ApplicationContext})...",
    InitialFunctions.AppName);

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
} else {
    app.UseExceptionHandler("/Error");
}

app.UseCors("CorsPolicy");
app.UseRouting();
app.UseEndpoints(endpoints => {
    endpoints.MapDefaultControllerRoute();
    endpoints.MapControllers();
    endpoints.MapHealthChecks("/hc",
        new HealthCheckOptions {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
    endpoints.MapHealthChecks("/liveness",
        new HealthCheckOptions { Predicate = r => r.Name.Contains("self") });
});

InitialFunctions.MigrateDbContext<ListContext>(app.Services,
    builder.Configuration, (context, servicesInside) => {
        var envInside = servicesInside.GetService<IWebHostEnvironment>();
        var loggerInside =
            servicesInside.GetService<ILogger<ListContextSeed>>();
        new ListContextSeed().SeedAsync(context, envInside, loggerInside)
            .Wait();
    });

Log.Information("Starting web host ({ApplicationContext})...",
    InitialFunctions.AppName);
app.Run();