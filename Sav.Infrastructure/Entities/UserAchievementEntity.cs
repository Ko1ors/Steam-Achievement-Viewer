using System.ComponentModel.DataAnnotations.Schema;

namespace Sav.Infrastructure.Entities
{
    [Table("UserAchievements")]
    public class UserAchievementEntity : BaseEntity
    {
        public string UserId { get; set; } = null!;

        public string AppID { get; set; } = null!;

        public string Apiname { get; set; } = null!;

        public string UnlockTime { get; set; } = null!;

        public virtual UserEntity User { get; set; } = null!;

        public virtual AchievementEntity Achievement { get; set; } = null!;

        public override object[] GetKeys()
        {
            return new object[] { UserId, AppID, Apiname };
        }
    }
}
