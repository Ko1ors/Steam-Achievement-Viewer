using Sav.Common.Models.DTOs;
using Sav.Infrastructure.Entities;

namespace Sav.Common.Models
{
    public class CompletedGameComposite
    {
        public UserGameDto UserGame { get; set; }

        public GameDto Game { get; set; }

        public IEnumerable<AchievementDto> Achievements { get; set; }

        public DateTime CompletedAt { get; set; }

        public string HoursPlayed => UserGame.HoursOnRecord ?? "0";

        public IEnumerable<AchievementDto> PreviewAchievements => Achievements.OrderByDescending(ua => ua.Percent).Take(10);

        public int PreviewCount => Achievements.Count() - PreviewAchievements.Count();

        public string PreviewCountString => PreviewCount.ToString();

        public string LogoUrl => $"https://cdn.cloudflare.steamstatic.com/steam/apps/{Game.AppID}/header.jpg";


        public CompletedGameComposite(UserGameEntity userGame)
        {
            UserGame = new UserGameDto(userGame);
            Game = new GameDto(userGame.Game);
            Achievements = userGame.UserAchievements.Select(ua => new AchievementDto(ua));
            CompletedAt = userGame.UserAchievements.Max(ua => ua.UnlockTime);
        }
    }
}
