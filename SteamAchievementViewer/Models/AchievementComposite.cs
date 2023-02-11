using System;

namespace SteamAchievementViewer.Models
{
    public class AchievementComposite
    {
        public string GameName { get; set; }
        
        public string GameIcon { get; set; }

        public string IconClosed { get; set; } = null!;

        public string IconOpen { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Apiname { get; set; } = null!;

        public string Description { get; set; } = null!;

        public float Percent { get; set; }

        public bool Unlocked { get; set; }

        public DateTime UnlockTime { get; set; }
    }
}
