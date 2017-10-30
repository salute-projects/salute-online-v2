using Microsoft.EntityFrameworkCore;
using SaluteOnline.Domain.Domain.EF;

namespace SaluteOnline.API.DAL
{
    public class SaluteOnlineDbContext : DbContext
    {
        public SaluteOnlineDbContext() {}
        public SaluteOnlineDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Club> Clubs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Club>().ToTable("Club");
        }
    }
}
