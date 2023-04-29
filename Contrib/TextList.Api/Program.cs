using RecAll.Contrib.TextList.Api;

var builder = WebApplication.CreateBuilder(args);

builder.AddCustomConfiguration();
// Console.WriteLine(builder.Configuration["ConnectionStrings:TextListContext"]);
builder.AddCustomDatabase();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.ApplyDatabaseMigration();

app.Run();