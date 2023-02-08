using Microsoft.EntityFrameworkCore;
using Sav.Infrastructure.Entities;

namespace Sav.Infrastructure
{
    public class SteamContext : DbContext
    {
        public DbSet<GameEntity> Games { get; set; } = null!;

        public DbSet<UserGameEntity> UserGames { get; set; } = null!;

        public DbSet<UserEntity> Users { get; set; }

        public DbSet<UserAchievementEntity> UserAchievements { get; set; }

        public DbSet<AchievementEntity> Achievements { get; set; }

        private readonly string _dbPath;
        
        public SteamContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseLazyLoadingProxies().UseSqlite($"Data Source=sav.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // TODO: check if apiname is unique, so composite key can be removed
            modelBuilder.Entity<AchievementEntity>().HasKey(a => new { a.AppID, a.Apiname });
            modelBuilder.Entity<UserAchievementEntity>().HasKey(ua => new { ua.UserId, ua.AppID, ua.Apiname });
            modelBuilder.Entity<UserGameEntity>().HasKey(ug => new { ug.UserId, ug.AppID });

            modelBuilder.Entity<GameEntity>().HasMany(g => g.UserGames).WithOne(ug => ug.Game);
            modelBuilder.Entity<UserEntity>().HasMany(u => u.UserGames).WithOne(ug => ug.User);
            modelBuilder.Entity<UserEntity>().HasMany(u => u.UserAchievements).WithOne(ua => ua.User);
            modelBuilder.Entity<AchievementEntity>().HasMany(a => a.UserAchievements).WithOne(ua => ua.Achievement);
            modelBuilder.Entity<GameEntity>().HasMany(g => g.Achievements).WithOne(a => a.Game);
        }
    }
}
