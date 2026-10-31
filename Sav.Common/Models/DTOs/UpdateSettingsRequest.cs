namespace Sav.Common.Models.DTOs
{
    public class UpdateSettingsRequest
    {
        public string? SteamId { get; set; }

        public string? SteamApiKey { get; set; }

        public int AutoUpdateIntervalMinutes { get; set; }

        public bool UpdateOnlyRecentGames { get; set; }
    }
}
