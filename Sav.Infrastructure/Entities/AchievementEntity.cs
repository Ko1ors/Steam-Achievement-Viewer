using System.ComponentModel.DataAnnotations.Schema;

namespace Sav.Infrastructure.Entities
{
    [Table("Achievements")]
    public class AchievementEntity : BaseEntity
    {
        public string IconClosed { get; set; } = null!;

        public string IconOpen { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Apiname { get; set; } = null!;

        public string Description { get; set; } = null!;

        public float Percent { get; set; }

        public string AppID { get; set; } = null!;

        public virtual GameEntity Game { get; set; } = null!;

        public virtual ICollection<UserAchievementEntity> UserAchievements { get; set; } = null!;

        public override object[] GetKeys()
        {
            return new object[] { AppID, Apiname };
        }
    }
}
