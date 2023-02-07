using Microsoft.EntityFrameworkCore;
using Sav.Infrastructure.Entities;

namespace Sav.Infrastructure
{
    public class SteamContext : DbContext
    {
        public DbSet<Game> Games { get; set; } = null!;

        public DbSet<UserGame> UserGames { get; set; } = null!;

        public DbSet<User> Users { get; set; }

        public DbSet<UserAchievement> UserAchievements { get; set; }

        public DbSet<Achievement> Achievements { get; set; }

        private readonly string _dbPath;
        
        public SteamContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseLazyLoadingProxies().UseSqlite($"Data Source=sav.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // TODO: check if apiname is unique, so composite key can be removed
            modelBuilder.Entity<Achievement>().HasKey(a => new { a.AppID, a.Apiname });
            modelBuilder.Entity<UserAchievement>().HasKey(ua => new { ua.UserId, ua.AppID, ua.Apiname });
            modelBuilder.Entity<UserGame>().HasKey(ug => new { ug.UserId, ug.AppID });

            modelBuilder.Entity<Game>().HasMany(g => g.UserGames).WithOne(ug => ug.Game);
            modelBuilder.Entity<User>().HasMany(u => u.UserGames).WithOne(ug => ug.User);
            modelBuilder.Entity<User>().HasMany(u => u.UserAchievements).WithOne(ua => ua.User);
            modelBuilder.Entity<Achievement>().HasMany(a => a.UserAchievements).WithOne(ua => ua.Achievement);
            modelBuilder.Entity<Game>().HasMany(g => g.Achievements).WithOne(a => a.Game);
        }
    }
}
