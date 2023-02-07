using SteamAchievementViewer.Models;
using System.Collections.Generic;
using System.Linq;

namespace SteamAchievementViewer.Services
{
    public class GameAchievementsService : IGameAchievementsService
    {
        private readonly ISteamService _steamService;

        public GameAchievementsService(ISteamService steamService)
        {
            _steamService = steamService;
        }

        public IEnumerable<AchievementGameInfo> GetAchievementGameInfos()
        {
            List<AchievementGameInfo> achievements = new List<AchievementGameInfo>();
            _steamService.GamesList.Games.Game.ForEach(g => achievements.AddRange(g.Achievements.Achievement.Select(a => new AchievementGameInfo(a, g))));
            return achievements;
        }

        public IEnumerable<AchievementGameInfo> GetClosestAchievements(int page, int count)
        {
            return GetAchievementGameInfos().OrderByDescending(e => e.Percent).Where(e => e.Closed == "0").Skip((page - 1) * count).Take(count);
        }

        public Achievements GetClosestAchievements(string appid)
        {
            return new Achievements
            {
                Achievement = _steamService.GamesList.Games.Game.Find(e => e.AppID == appid)?.Achievements.Achievement.Where(e => e.Closed == "0").OrderByDescending(e => e.Percent).ToList()
            };
        }

        public int GetCompletedAchievementsCount()
        {
            return _steamService.GamesList.Games.Game.Sum(g => g.Achievements.Achievement.Count(a => a.Closed == "1"));
        }

        public IEnumerable<Game> GetIncompleteGames(int page, int count)
        {
            return _steamService.GamesList.Games.Game.Where(a => a.Achievements.Achievement.Any(e => e.Closed == "0")).Skip((page - 1) * count).Take(count);
        }

        public IEnumerable<AchievementGameInfo> GetLatestAchievements(int page, int count)
        {
            return GetAchievementGameInfos().OrderByDescending(e => e.UnlockTime).Where(e => e.Closed == "1").Skip((page - 1) * count).Take(count);
        }

        public IEnumerable<AchievementGameInfo> GetRarestAchievements(int page, int count)
        {
            return GetAchievementGameInfos().OrderBy(e => e.Percent).Where(e => e.Closed == "0" && e.Percent != -1).Skip((page - 1) * count).Take(count);
        }

        public int GetTotalAchievementsCount()
        {
            return _steamService.GamesList.Games.Game.Sum(g => g.Achievements.Achievement.Count());
        }
    }
}
