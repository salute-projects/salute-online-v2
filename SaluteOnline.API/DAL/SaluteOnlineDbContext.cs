using Microsoft.EntityFrameworkCore;
using SaluteOnline.Domain.Domain.EF;
using SaluteOnline.Domain.Domain.EF.LinkEntities;

namespace SaluteOnline.API.DAL
{
    public class SaluteOnlineDbContext : DbContext
    {
        public SaluteOnlineDbContext() {}
        public SaluteOnlineDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Club> Clubs { get; set; }
        public DbSet<Player> Players { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("User").HasIndex(t => t.SubjectId).IsUnique();
            modelBuilder.Entity<Club>().ToTable("Club");

            modelBuilder.Entity<ClubUserAdministrator>().HasKey(t => new {t.ClubId, t.UserId});
            modelBuilder.Entity<ClubUserAdministrator>()
                .HasOne(t => t.User)
                .WithMany(t => t.ClubsAdministrated)
                .HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ClubUserAdministrator>()
                .HasOne(t => t.Club)
                .WithMany(t => t.Administrators)
                .HasForeignKey(t => t.ClubId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<MembershipRequest>()
                .HasOne(t => t.Club)
                .WithMany(t => t.MembershipRequests)
                .HasForeignKey(t => t.ClubId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Club>().HasMany(t => t.Players).WithOne(t => t.Club).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
