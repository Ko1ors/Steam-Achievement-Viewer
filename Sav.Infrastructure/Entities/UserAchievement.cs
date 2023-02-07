namespace Sav.Infrastructure.Entities
{
    public class UserAchievement : BaseEntity
    {
        public string UserId { get; set; } = null!;

        public string AppID { get; set; } = null!;

        public string Apiname { get; set; } = null!;

        public string UnlockTime { get; set; } = null!;

        public virtual User User { get; set; } = null!;

        public virtual Achievement Achievement { get; set; } = null!;
    }
}
