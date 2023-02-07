using SteamAchievementViewer.Models;
using System.Collections.Generic;

namespace SteamAchievementViewer.Services
{
    public interface IGameAchievementsService
    {
        IEnumerable<AchievementGameInfo> GetAchievementGameInfos();

        IEnumerable<AchievementGameInfo> GetRarestAchievements(int page = 1, int count = 100);

        IEnumerable<AchievementGameInfo> GetLatestAchievements(int page = 1, int count = 100);

        IEnumerable<AchievementGameInfo> GetClosestAchievements(int page = 1, int count = 100);

        Achievements GetClosestAchievements(string appid);

        IEnumerable<Game> GetIncompleteGames(int page = 1, int count = 100);

        int GetCompletedAchievementsCount();

        int GetTotalAchievementsCount();
    }
}
