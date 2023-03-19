namespace Sav.Common.Models
{
    public class CompletionGameComposite
    {
        public string AppID { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? GameIcon { get; set; } = null!;

        public string Logo { get; set; } = null!;

        public string? GameLogoSmall { get; set; }

        public string StoreLink { get; set; } = null!;

        public string? HoursLast2Weeks { get; set; }

        public string? HoursOnRecord { get; set; }

        public string? GlobalStatsLink { get; set; }

        public float AverageAchievementsPercentage { get; set; }

        public float LowestAchievementPercentage { get; set; }
    }
}
