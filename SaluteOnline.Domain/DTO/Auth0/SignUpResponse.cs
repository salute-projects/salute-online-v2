using Newtonsoft.Json;

namespace SaluteOnline.Domain.DTO.Auth0
{
    public class SignUpResponse
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("email_verified")]
        public bool EmailVerified { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("user_metadata")]
        public dynamic UserMetadata { get; set; }
    }
}
