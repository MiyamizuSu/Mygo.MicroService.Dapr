using Dapr.Client;
using Dapr.Extensions.Configuration;

namespace RecAll.Contrib.TextList.Api;

public static class ProgramExtensions {
    public static readonly string AppName = typeof(ProgramExtensions).Namespace;

    public static void AddCustomConfiguration(
        this WebApplicationBuilder builder) {
        builder.Configuration.AddDaprSecretStore("recall-secretstore",
            new DaprClientBuilder().Build());
    }
}