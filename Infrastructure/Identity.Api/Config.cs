using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace RecAll.Infrastructure.Identity.Api;

public class Config {
    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource> {
            new IdentityResources.OpenId(), new IdentityResources.Profile()
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope> {
            new("List", "List"), new("TextList", "Text List")
        };

    // TODO Swagger Client
    public static IEnumerable<Client>
        GetClients(Dictionary<string, string> clientUrlDict) =>
        new List<Client> {
            new() {
                ClientId = "mvc",
                ClientName = "MVC Client",
                ClientSecrets =
                    new List<Secret> { new Secret("secret".Sha256()) },
                ClientUri = $"{clientUrlDict["Mvc"]}",
                AllowedGrantTypes = GrantTypes.Hybrid,
                AllowAccessTokensViaBrowser = false,
                RequirePkce = false,
                RequireConsent = false,
                AllowOfflineAccess = true,
                AlwaysIncludeUserClaimsInIdToken = true,
                RedirectUris =
                    new List<string> { $"{clientUrlDict["Mvc"]}/signin-oidc" },
                PostLogoutRedirectUris =
                    new List<string> {
                        $"{clientUrlDict["Mvc"]}/signout-callback-oidc"
                    },
                AllowedScopes =
                    new List<string> {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess
                    },
                AccessTokenLifetime = 60 * 60 * 2,
                IdentityTokenLifetime = 60 * 60 * 2
            },
            new() {
                ClientId = "blazor",
                ClientName = "Blazor Client",
                ClientSecrets =
                    new List<Secret> { new Secret("secret".Sha256()) },
                ClientUri = $"{clientUrlDict["Blazor"]}",
                AllowedGrantTypes = GrantTypes.Hybrid,
                AllowAccessTokensViaBrowser = false,
                RequirePkce = false,
                RequireConsent = false,
                AllowOfflineAccess = true,
                AlwaysIncludeUserClaimsInIdToken = true,
                RedirectUris =
                    new List<string> {
                        $"{clientUrlDict["Blazor"]}/signin-oidc"
                    },
                PostLogoutRedirectUris =
                    new List<string> {
                        $"{clientUrlDict["Blazor"]}/signout-callback-oidc"
                    },
                AllowedScopes = new List<string> {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.OfflineAccess
                },
                AccessTokenLifetime = 60 * 60 * 2,
                IdentityTokenLifetime = 60 * 60 * 2
            }
        };
}