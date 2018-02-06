using System.Collections.Generic;
using IdentityServer4.Models;

namespace SaluteOnline.IdentityServer.Domain
{
    public class WebApplicationClientSettings
    {
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientUri { get; set; }
        public bool AllowOfflineAccess { get; set; }
        public bool RequireConsent { get; set; }
        public bool AllowAccessTokensViaBrowser { get; set; }
        public ICollection<string> RedirectUris { get; set; }
        public ICollection<string> PostLogoutRedirectUris { get; set; }
        public TokenUsage RefreshTokenUsage { get; set; }
        public bool UpdateAccessTokenClaimsOnRefresh { get; set; }
        public TokenExpiration RefreshTokenExpiration { get; set; }
        public ICollection<string> AllowedScopes { get; set; }
        public int AccessTokenLifetime { get; set; }
        public AccessTokenType AccessTokenType { get; set; }
        public int IdentityTokenLifetime { get; set; }
    }
}
