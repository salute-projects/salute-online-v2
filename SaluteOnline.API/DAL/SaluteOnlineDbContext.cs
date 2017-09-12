using Microsoft.EntityFrameworkCore;

namespace SaluteOnline.API.DAL
{
    public class SaluteOnlineDbContext : DbContext
    {
        public SaluteOnlineDbContext() {}

        public SaluteOnlineDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}
