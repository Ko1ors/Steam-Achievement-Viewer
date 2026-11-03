using Sav.Infrastructure.Entities;

namespace Sav.Common.Models.DTOs
{
    public class UserProfileDto
    {
        public string SteamID64 { get; set; } = null!;

        public string SteamID { get; set; } = null!;

        public string? CustomURL { get; set; }

        public string? AvatarIcon { get; set; }

        public string? AvatarMedium { get; set; }

        public string? AvatarFull { get; set; }

        public string? AvatarFrame { get; set; }

        public string? OnlineState { get; set; }

        public string? Headline { get; set; }

        public string? RealName { get; set; }

        public string? Location { get; set; }

        public DateTime? MemberSince { get; set; }

        public int GamesCount { get; set; }

        public int TotalAchievementsCount { get; set; }

        public int CompletedAchievementsCount { get; set; }

        public UserProfileDto(UserEntity user, int gamesCount, int totalAchievementsCount, int completedAchievementsCount)
        {
            SteamID64 = user.SteamID64;
            SteamID = user.SteamID;
            CustomURL = user.CustomURL;
            AvatarIcon = user.AvatarIcon;
            AvatarMedium = user.AvatarMedium;
            AvatarFull = user.AvatarFull;
            AvatarFrame = user.AvatarFrame;
            OnlineState = user.OnlineState;
            Headline = string.IsNullOrWhiteSpace(user.Headline) ? null : user.Headline;
            RealName = string.IsNullOrWhiteSpace(user.Realname) ? null : user.Realname;
            Location = string.IsNullOrWhiteSpace(user.Location) ? null : user.Location;
            MemberSince = user.MemberSince == default ? null : user.MemberSince;
            GamesCount = gamesCount;
            TotalAchievementsCount = totalAchievementsCount;
            CompletedAchievementsCount = completedAchievementsCount;
        }
    }
}
