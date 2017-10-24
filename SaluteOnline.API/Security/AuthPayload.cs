using Newtonsoft.Json;

namespace SaluteOnline.API.Security
{
    internal class AuthPayload
    {
        [JsonProperty(PropertyName = "iss")]
        public string Issuer { get; set; }

        [JsonProperty(PropertyName = "sub")]
        public string Subject { get; set; }

        [JsonProperty(PropertyName = "exp")]
        public string Expiration { get; set; }

        [JsonProperty(PropertyName = "aud")]
        public string Audience { get; set; }
    }
}
