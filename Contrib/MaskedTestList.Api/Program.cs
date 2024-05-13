using Dapr.Client;
using Dapr.Extensions.Configuration;
using RecAll.Contrib.MaskedTestList.Api;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Configuration.AddDaprSecretStore("recall-secretstore", new DaprClientBuilder().Build());
// Configure the HTTP request pipeline.
builder.AddCustomDatabase();
var app = builder.Build();
app.ApplyDatabaseMigration();
app.Run();   

