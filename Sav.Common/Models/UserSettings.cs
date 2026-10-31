namespace Sav.Common.Models
{
    public class UserSettings
    {
        public const int DefaultAutoUpdateIntervalMinutes = 60;

        public string? SteamID { get; set; }

        public string? SteamApiKey { get; set; }

        public int AutoUpdateIntervalMinutes { get; set; } = DefaultAutoUpdateIntervalMinutes;

        public bool UpdateOnlyRecentGames { get; set; }
    }
}
