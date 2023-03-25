using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sav.Infrastructure.Entities
{
    [Table("Games")]
    public class GameEntity : BaseEntity
    {
        [Key]
        public string AppID { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? GameIcon { get; set; } = null!;

        public string Logo { get; set; } = null!;

        public string? GameLogoSmall { get; set; }

        public string StoreLink { get; set; } = null!;

        public string? GlobalStatsLink { get; set; }

        public virtual ICollection<UserGameEntity> UserGames { get; set; } = null!;

        public virtual ICollection<AchievementEntity> Achievements { get; set; } = null!;

        public override object[] GetKeys()
        {
            return new object[] { AppID };
        }
    }
}
