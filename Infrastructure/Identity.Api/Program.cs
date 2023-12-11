using RecAll.Infrastructure.Identity.Api;

var builder = WebApplication.CreateBuilder(args);

builder.AddCustomConfiguration();
builder.AddCustomHealthChecks();
builder.Services.AddRazorPages();
builder.AddCustomIdentityServer();

var app = builder.Build();

app.UseIdentityServer();
app.UseCookiePolicy(
    new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

await app.ApplyDatabaseMigrationAsync();
app.Run();