namespace Sav.Common.Models
{
    public class UserSettings
    {
        public const int DefaultAutoUpdateIntervalMinutes = 60;

        public string? SteamID { get; set; }

        public string? SteamApiKey { get; set; }

        public int AutoUpdateIntervalMinutes { get; set; } = DefaultAutoUpdateIntervalMinutes;

        /// <summary>When the background auto update last ran; kept out of the settings API on purpose.</summary>
        public DateTime? LastAutoUpdateUtc { get; set; }
    }
}
