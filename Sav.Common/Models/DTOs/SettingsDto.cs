namespace Sav.Common.Models.DTOs
{
    public class SettingsDto
    {
        public string? SteamId { get; set; }

        public string? SteamApiKey { get; set; }

        public int AutoUpdateIntervalMinutes { get; set; }
    }
}
