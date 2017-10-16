namespace SaluteOnline.API.Security
{
    public class Auth0Settings
    {
        public string Domain { get; set; }
        public string ApiIdentifier { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Audience { get; set; }
    }
}
