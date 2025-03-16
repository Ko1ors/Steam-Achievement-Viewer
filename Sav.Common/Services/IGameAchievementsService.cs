using Sav.Common.Models;
using Sav.Infrastructure.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sav.Common.Services
{
    public interface IGameAchievementsService
    {
        IEnumerable<AchievementComposite> GetAchievementComposites();

        IEnumerable<AchievementComposite> GetRarestAchievements(int page = 1, int count = 100);

        IEnumerable<AchievementComposite> GetLatestAchievements(int page = 1, int count = 100);

        IEnumerable<AchievementComposite> GetClosestAchievements(string steamId, int take = 25, int skip = 0);

        IEnumerable<AchievementComposite> GetGameClosestAchievements(string steamId, string appid);

        IEnumerable<CompletionGameComposite> GetEasiestGamesToComplete(int page = 1, int count = 100);

        PagedResult<CompletionGameComposite> GetPagedEasiestGamesToComplete(int page = 1, int count = 100);

        Task<PagedResult<CompletionGameComposite>> GetPagedEasiestGamesToCompleteAsync(int page = 1, int count = 100);

        IEnumerable<GameEntity> GetIncompleteGames(string steamId, int take = 25, int skip = 0);

        Task<PagedResult<CompletedGameComposite>> GetPagedCompletedGamesAsync(string steamId, int take = 25, int skip = 0);

        int GetCompletedAchievementsCount();

        int GetTotalAchievementsCount();
    }
}
