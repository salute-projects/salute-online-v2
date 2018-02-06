using System.Collections.Generic;

namespace SaluteOnline.IdentityServer.Domain
{
    public class ApiClientSettings
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public ICollection<string> ClaimTypes { get; set; }
    }
}
