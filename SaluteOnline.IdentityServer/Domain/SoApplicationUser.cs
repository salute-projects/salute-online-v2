using Microsoft.AspNetCore.Identity;
using SaluteOnline.Domain.DTO;

namespace SaluteOnline.IdentityServer.Domain
{
    public class SoApplicationUser : IdentityUser
    {
        public Roles Role { get; set; }
        public long UserId { get; set; }
        public string AvatarUrl { get; set; }
    }
}
