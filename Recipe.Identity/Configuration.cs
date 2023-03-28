using System;
using System.Collections.Generic;
using System.Linq;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityModel;

namespace Recipe.Identity
{
    public static class Configuration
    {
        public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new ApiScope("RecipeWebAPI", "Web API")
        };

        public static IEnumerable<ApiResource> ApiResources =>
        new List<ApiResource>
        {
            new ApiResource("RecipeWebAPI", "Web API", new[]
            { JwtClaimTypes.Name})
            {
                Scopes = {"RecipeWebAPI"}
            }
        };

        public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        };

        public static IEnumerable<Client> Clients =>
        new List<Client>
        {
            new Client
            {
                ClientId = "recipe-web-api",
                ClientName = "Recipe Web",
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                RequireClientSecret = false,
                RefreshTokenUsage = TokenUsage.OneTimeOnly,
                AllowOfflineAccess = true,
                RedirectUris =
                {
                    "https://.../signin-oidc"
                },
                AllowedCorsOrigins =
                {
                    "https://..."
                },
                PostLogoutRedirectUris =
                {
                    "https://.../signout-callback-oidc"
                },
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "RecipeWebAPI"
                },
                AllowAccessTokensViaBrowser = true
            },

            new Client
            {
                ClientId = "client_mvc",
                ClientSecrets = {new Secret("client_secret_mvc".ToSha256())},
                ClientName = "Recipe MVC",
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                RefreshTokenUsage = TokenUsage.OneTimeOnly,
                AllowOfflineAccess = true,
                RedirectUris =
                {
                    "https://localhost:7001/signin-oidc"
                },
                // AllowedCorsOrigins =
                // {
                //     "https://localhost:7001"
                // },
                PostLogoutRedirectUris =
                {
                    "https://localhost:7001/signout-callback-oidc"
                },
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "RecipeWebAPI"
                },
                AllowAccessTokensViaBrowser = true
            }
        };
    }
}