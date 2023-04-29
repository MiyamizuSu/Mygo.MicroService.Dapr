using RecAll.Contrib.TextList.Api;

var builder = WebApplication.CreateBuilder(args);

builder.AddCustomConfiguration();
// Console.WriteLine(builder.Configuration["ConnectionStrings:TextListContext"]);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();