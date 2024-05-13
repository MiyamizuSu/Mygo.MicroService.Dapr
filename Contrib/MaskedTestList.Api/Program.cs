using Dapr.Client;
using Dapr.Extensions.Configuration;
using RecAll.Contrib.MaskedTestList.Api;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Configuration.AddDaprSecretStore("recall-secretstore", new DaprClientBuilder().Build());
builder.Services.AddControllers();
// Configure the HTTP request pipeline.
builder.AddCustomDatabase();
builder.addCustomSwagger();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.useCustomSwagger();
    app.MapGet("/", () => Results.LocalRedirect("~/swagger"));
}
app.MapControllers();
app.ApplyDatabaseMigration();
app.Run();   

