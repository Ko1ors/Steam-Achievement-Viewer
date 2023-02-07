using System.ComponentModel.DataAnnotations;

namespace Sav.Infrastructure.Entities
{
    public class User : BaseEntity
    {
        [Key]
        public string SteamID { get; set; } = null!;

        public string OnlineState { get; set; } = null!;

        public string StateMessage { get; set; } = null!;

        public string PrivacyState { get; set; } = null!;

        public int VisibilityState { get; set; }

        public string AvatarIcon { get; set; } = null!;

        public string AvatarMedium { get; set; } = null!;

        public string AvatarFull { get; set; } = null!;

        public int VacBanned { get; set; }

        public string TradeBanState { get; set; } = null!;

        public int IsLimitedAccount { get; set; }

        public string CustomURL { get; set; } = null!;

        public DateTime MemberSince { get; set; }

        public double HoursPlayed2Wk { get; set; }

        public string Headline { get; set; } = null!;

        public string Location { get; set; } = null!;

        public string Realname { get; set; } = null!;

        public string Summary { get; set; } = null!;

        public virtual ICollection<UserGame> UserGames { get; set; } = null!;

        public virtual ICollection<UserAchievement> UserAchievements { get; set; } = null!;
    }
}
