using System.Collections.Generic;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;

namespace SaluteOnline.IdentityServer.Constants
{
    public class DefaultInfrastructure
    {
        public static IEnumerable<ApiResource> GetApiResources(IConfigurationSection apiClientSettings)
        {
            return new List<ApiResource>
            {
                new ApiResource(apiClientSettings.GetValue<string>("Name"),
                    apiClientSettings.GetValue<string>("DisplayName"),
                    apiClientSettings.GetSection("ClaimTypes").Get<List<string>>())
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<Client> GetClients(IConfigurationSection webApplicationClientSettings,
            IConfigurationSection resourceOwnerClientSettings)
        {
            var webApplicationClient = new Client
            {
                ClientId = webApplicationClientSettings.GetValue<string>(nameof(Client.ClientId)),
                ClientName = webApplicationClientSettings.GetValue<string>(nameof(Client.ClientName)),
                ClientSecrets = new List<Secret> { new Secret(webApplicationClientSettings.GetValue<string>(nameof(Client.ClientSecrets)).Sha256()) },
                ClientUri = webApplicationClientSettings.GetValue<string>(nameof(Client.ClientUri)),
                AllowedGrantTypes = GrantTypes.Implicit,
                AllowOfflineAccess = webApplicationClientSettings.GetValue<bool>(nameof(Client.AllowOfflineAccess)),
                AllowAccessTokensViaBrowser = webApplicationClientSettings.GetValue<bool>(nameof(Client.AllowAccessTokensViaBrowser)),
                RedirectUris = webApplicationClientSettings.GetSection(nameof(Client.RedirectUris)).Get<List<string>>(),
                PostLogoutRedirectUris = webApplicationClientSettings.GetSection(nameof(Client.PostLogoutRedirectUris)).Get<List<string>>(),
                RefreshTokenUsage = webApplicationClientSettings.GetValue<TokenUsage>(nameof(Client.RefreshTokenUsage)),
                UpdateAccessTokenClaimsOnRefresh = webApplicationClientSettings.GetValue<bool>(nameof(Client.UpdateAccessTokenClaimsOnRefresh)),
                RefreshTokenExpiration = webApplicationClientSettings.GetValue<TokenExpiration>(nameof(Client.RefreshTokenExpiration)),
                AllowedScopes = webApplicationClientSettings.GetSection(nameof(Client.AllowedScopes)).Get<List<string>>(),
                AccessTokenLifetime = webApplicationClientSettings.GetValue<int>(nameof(Client.AccessTokenLifetime)),
                AccessTokenType = webApplicationClientSettings.GetValue<AccessTokenType>(nameof(Client.AccessTokenType)),
                IdentityTokenLifetime = webApplicationClientSettings.GetValue<int>(nameof(Client.IdentityTokenLifetime)),
                RequireConsent = webApplicationClientSettings.GetValue<bool>(nameof(Client.RequireConsent)),
                AllowedCorsOrigins = new List<string> { "http://localhost:4200" }
            };

            var resourceOwnerClient = new Client
            {
                ClientId = resourceOwnerClientSettings.GetValue<string>(nameof(Client.ClientId)),
                ClientName = resourceOwnerClientSettings.GetValue<string>(nameof(Client.ClientName)),
                ClientSecrets = new List<Secret> { new Secret(resourceOwnerClientSettings.GetValue<string>(nameof(Client.ClientSecrets)).Sha256()) },
                AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                AllowOfflineAccess = resourceOwnerClientSettings.GetValue<bool>(nameof(Client.AllowOfflineAccess)),
                RefreshTokenUsage = resourceOwnerClientSettings.GetValue<TokenUsage>(nameof(Client.RefreshTokenUsage)),
                UpdateAccessTokenClaimsOnRefresh = resourceOwnerClientSettings.GetValue<bool>(nameof(Client.UpdateAccessTokenClaimsOnRefresh)),
                AlwaysSendClientClaims = resourceOwnerClientSettings.GetValue<bool>(nameof(Client.AlwaysSendClientClaims)),
                AllowedScopes = resourceOwnerClientSettings.GetSection(nameof(Client.AllowedScopes)).Get<List<string>>(),
                AccessTokenLifetime = resourceOwnerClientSettings.GetValue<int>(nameof(Client.AccessTokenLifetime)),
                AccessTokenType = resourceOwnerClientSettings.GetValue<AccessTokenType>(nameof(Client.AccessTokenType)),
                IdentityTokenLifetime = resourceOwnerClientSettings.GetValue<int>(nameof(Client.IdentityTokenLifetime))
            };
            
            return new List<Client> { webApplicationClient, resourceOwnerClient };
        }
    }
}
