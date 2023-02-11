using Sav.Infrastructure.Entities;

namespace Sav.Common.Models
{
    public class EntityComposite
    {
        public GameEntity Game { get; set; } = null!;

        public AchievementEntity Achievement { get; set; } = null!;

        public UserAchievementEntity? UserAchievement { get; set; }
    }
}
