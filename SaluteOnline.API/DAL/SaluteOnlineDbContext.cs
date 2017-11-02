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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("User");
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

            modelBuilder.Entity<ClubUserPlayers>().HasKey(t => new {t.ClubId, t.UserId});
            modelBuilder.Entity<ClubUserPlayers>()
                .HasOne(t => t.User)
                .WithMany(t => t.ClubsParticipated)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ClubUserPlayers>()
                .HasOne(t => t.Club)
                .WithMany(t => t.Players)
                .HasForeignKey(t => t.ClubId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
