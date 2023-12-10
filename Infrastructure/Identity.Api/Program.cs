using System.Reflection;
using Identity.Api;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;
configuration.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", false, true).AddEnvironmentVariables();

var connectionString =
    configuration.GetConnectionString("ApplicationDbContext");
var migrationAssemblyString =
    typeof(Program).GetTypeInfo().Assembly.GetName().Name;

services.AddControllersWithViews();

services.AddIdentityServer().AddTestUsers(TestUsers.Users)
    .AddConfigurationStore(o => o.ConfigureDbContext = ob =>
        ob.UseSqlServer(connectionString,
            sql => sql.MigrationsAssembly(migrationAssemblyString)))
    .AddOperationalStore(o => o.ConfigureDbContext = ob =>
        ob.UseSqlServer(connectionString,
            sql => sql.MigrationsAssembly(migrationAssemblyString)))
    .AddDeveloperSigningCredential();

var app = builder.Build();

app.UseIdentityServer();

app.UseCookiePolicy(
    new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(e => e.MapDefaultControllerRoute());

app.Run();