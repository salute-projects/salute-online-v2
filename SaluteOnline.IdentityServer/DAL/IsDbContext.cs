using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SaluteOnline.IdentityServer.Domain;

namespace SaluteOnline.IdentityServer.DAL
{
    public class IsDbContext : IdentityDbContext<SoApplicationUser>
    {
        public IsDbContext(DbContextOptions options) : base(options) {}
    }
}
