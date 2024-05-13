using Microsoft.EntityFrameworkCore;
using Polly;
using RecAll.Contrib.MaskedTestList.Api.Services;

namespace RecAll.Contrib.MaskedTestList.Api;

public static class ProgramExtensions
{
    public static void AddCustomDatabase(this WebApplicationBuilder builder) {
        builder.Services.AddDbContext<MaskedTextListContext>(options =>
            options.UseSqlServer(
                builder.Configuration["ConnectionStrings:MaskedTextListContext"]));
    }
    public static void ApplyDatabaseMigration(this WebApplication app) {
        // Apply database migration automatically. Note that this approach is not
        // recommended for production scenarios. Consider generating SQL scripts from
        // migrations instead.
        using var scope = app.Services.CreateScope();

        var retryPolicy = CreateRetryPolicy();
        var context =
            scope.ServiceProvider.GetRequiredService<MaskedTextListContext>();

        retryPolicy.Execute(context.Database.Migrate);
    }
    private static Policy CreateRetryPolicy() {
        return Policy.Handle<Exception>().WaitAndRetryForever(
            sleepDurationProvider: _ => TimeSpan.FromSeconds(5),
            onRetry: (exception, retry, _) => {
                Console.WriteLine(
                    "Exception {0} with message {1} detected during database migration (retry attempt {2})",
                    exception.GetType().Name, exception.Message, retry);
            });
    }
}