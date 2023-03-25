using Sav.Infrastructure.Entities;

namespace Sav.Common.Models
{
    public class CompletedGameComposite
    {
        public UserGameEntity UserGame { get; set; }

        public GameEntity Game { get; set; }

        public IEnumerable<AchievementEntity> Achievements { get; set; }

        public DateTime CompletedAt { get; set; }

        public string HoursPlayed => UserGame.HoursOnRecord ?? "0";

        public IEnumerable<AchievementEntity> PreviewAchievements => Achievements.OrderByDescending(ua => ua.Percent).Take(10);

        public int PreviewCount => Achievements.Count() - PreviewAchievements.Count();

        public string PreviewCountString => PreviewCount.ToString();

        public string LogoUrl => $"https://cdn.cloudflare.steamstatic.com/steam/apps/{Game.AppID}/header.jpg";
    }
}
