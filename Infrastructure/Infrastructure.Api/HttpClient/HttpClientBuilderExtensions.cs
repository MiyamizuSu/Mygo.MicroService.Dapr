namespace Infrastructure.Api.HttpClient;

public static class HttpClientBuilderExtensions {
    public static void AddCustomHttpClient(this WebApplicationBuilder builder) {
        builder.Services
            .AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    }
}