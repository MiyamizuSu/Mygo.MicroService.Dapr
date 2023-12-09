var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddControllersWithViews();

// services.AddIdentityServer()
//     .AddInMemoryIdentityResources(Config.IdentityResources)
//     .AddInMemoryApiScopes(Config.ApiScopes).AddInMemoryClients(Config.Clients)
//     .AddDeveloperSigningCredential();

var app = builder.Build();

app.UseIdentityServer();

app.UseCookiePolicy(
    new CookiePolicyOptions {MinimumSameSitePolicy = SameSiteMode.Lax});
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(e => e.MapDefaultControllerRoute());

app.Run();