using System.Collections.Generic;
using Newtonsoft.Json;

namespace SaluteOnline.HubService.Security
{
    public class AuthPayload
    {
        [JsonProperty(PropertyName = "nbf")]
        public string NotBefore { get; set; }

        [JsonProperty(PropertyName = "exp")]
        public string Expiration { get; set; }

        [JsonProperty(PropertyName = "iss")]
        public string Issuer { get; set; }

        [JsonProperty(PropertyName = "aud")]
        public IEnumerable<string> Audience { get; set; }

        [JsonProperty(PropertyName = "client_id")]
        public string ClientId { get; set; }

        [JsonProperty(PropertyName = "sub")]
        public string Subject { get; set; }

        [JsonProperty(PropertyName = "auth_time")]
        public string AuthenticationTime { get; set; }

        [JsonProperty(PropertyName = "idp")]
        public string Idp { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string UserName { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "role")]
        public string Role { get; set; }

        [JsonProperty(PropertyName = "picture")]
        public string Picture { get; set; }

        [JsonProperty(PropertyName = "UserId")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "scope")]
        public IEnumerable<string> Scope { get; set; }

        [JsonProperty(PropertyName = "amr")]
        public IEnumerable<string> AuthenticationMethod { get; set; }
    }
}
