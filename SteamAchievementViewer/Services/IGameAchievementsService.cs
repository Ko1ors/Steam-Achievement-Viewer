using Sav.Common.Models;
using Sav.Infrastructure.Entities;
using System.Collections.Generic;

namespace SteamAchievementViewer.Services
{
    public interface IGameAchievementsService
    {
        IEnumerable<AchievementComposite> GetAchievementComposites();

        IEnumerable<AchievementComposite> GetRarestAchievements(int page = 1, int count = 100);

        IEnumerable<AchievementComposite> GetLatestAchievements(int page = 1, int count = 100);

        IEnumerable<AchievementComposite> GetClosestAchievements(int page = 1, int count = 100);

        IEnumerable<AchievementComposite> GetGameClosestAchievements(string appid);

        IEnumerable<GameEntity> GetIncompleteGames(int page = 1, int count = 100);

        int GetCompletedAchievementsCount();

        int GetTotalAchievementsCount();
    }
}
