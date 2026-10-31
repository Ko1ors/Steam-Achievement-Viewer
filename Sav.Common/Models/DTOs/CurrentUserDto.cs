namespace Sav.Common.Models.DTOs
{
    public class CurrentUserDto
    {
        public bool IsLoggedIn { get; set; }

        public string? SteamId { get; set; }

        public UserProfileDto? Profile { get; set; }

        public int QueueSize { get; set; }
    }
}
