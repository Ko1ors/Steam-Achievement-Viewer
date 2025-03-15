using Sav.Infrastructure.Entities;

namespace Sav.Common.Models.DTOs
{
    public class AchievementDto
    {
        public string IconClosed { get; set; } = null!;

        public string IconOpen { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Apiname { get; set; } = null!;

        public string Description { get; set; } = null!;

        public float Percent { get; set; }

        public string AppID { get; set; } = null!;

        public DateTime? UnlockTime { get; set; }

        public AchievementDto(AchievementEntity achievement)
        {
            IconClosed = achievement.IconClosed;
            IconOpen = achievement.IconOpen;
            Name = achievement.Name;
            Apiname = achievement.Apiname;
            Description = achievement.Description;
            Percent = achievement.Percent;
            AppID = achievement.AppID;
            //UnlockTime = achievement.UserAchievements.Max(ua => ua.UnlockTime);
        }

        public AchievementDto(UserAchievementEntity userAchievement)
        {
            IconClosed = userAchievement.Achievement.IconClosed;
            IconOpen = userAchievement.Achievement.IconOpen;
            Name = userAchievement.Achievement.Name;
            Apiname = userAchievement.Apiname;
            Description = userAchievement.Achievement.Description;
            Percent = userAchievement.Achievement.Percent;
            AppID = userAchievement.AppID;
            UnlockTime = userAchievement.UnlockTime;
        }
    }
}
