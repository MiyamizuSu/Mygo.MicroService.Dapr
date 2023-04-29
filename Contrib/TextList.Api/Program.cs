using RecAll.Contrib.TextList.Api;

var builder = WebApplication.CreateBuilder(args);

builder.AddCustomConfiguration();
// Console.WriteLine(builder.Configuration["ConnectionStrings:TextListContext"]);
builder.AddCustomSwagger();
builder.AddCustomApplicationServices();
builder.AddCustomDatabase();

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
    app.UseCustomSwagger();
}

app.MapGet("/", () => Results.LocalRedirect("~/swagger"));
app.MapControllers();

app.ApplyDatabaseMigration();

app.Run();