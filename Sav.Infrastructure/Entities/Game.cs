using System.ComponentModel.DataAnnotations;

namespace Sav.Infrastructure.Entities
{
    public class Game : BaseEntity
    {
        [Key]
        public string AppID { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string GameIcon { get; set; } = null!;

        public string Logo { get; set; } = null!;

        public string GameLogoSmall { get; set; } = null!;

        public string StoreLink { get; set; } = null!;

        public string HoursLast2Weeks { get; set; } = null!;

        public string HoursOnRecord { get; set; } = null!;

        public string GlobalStatsLink { get; set; } = null!;

        public virtual ICollection<UserGame> UserGames { get; set; } = null!;

        public virtual ICollection<Achievement> Achievements { get; set; } = null!;
    }
}
