using HealthChecks.UI.Client;
using Infrastructure.Api;
using RecAll.Contrib.TextList.Api;

var builder = WebApplication.CreateBuilder(args);

builder.AddCustomConfiguration();
// Console.WriteLine(builder.Configuration["ConnectionStrings:TextListContext"]);
builder.AddCustomSerilog();
builder.AddCustomSwagger();
builder.AddCustomHealthChecks();
builder.AddCustomApplicationServices();
builder.AddCustomDatabase();
builder.AddInvalidModelStateResponseFactory();

builder.Services.AddDaprClient();
builder.Services.AddControllers().AddDapr();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
    app.UseCustomSwagger();
}

app.MapGet("/", () => Results.LocalRedirect("~/swagger"));
app.UseCloudEvents();
app.MapControllers();
app.MapSubscribeHandler();
app.MapCustomHealthChecks(
    responseWriter: UIResponseWriter.WriteHealthCheckUIResponse);

app.ApplyDatabaseMigration();

app.Run();