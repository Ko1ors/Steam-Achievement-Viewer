namespace Sav.Infrastructure.Entities
{
    public class UserGame : BaseEntity
    {
        public string UserId { get; set; } = null!;

        public string AppID { get; set; } = null!;

        public string StatsLink { get; set; } = null!;

        public virtual User User { get; set; } = null!;

        public virtual Game Game { get; set; } = null!;
    }
}
