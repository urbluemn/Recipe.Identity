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
                    "https://.../signout-oidc"
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