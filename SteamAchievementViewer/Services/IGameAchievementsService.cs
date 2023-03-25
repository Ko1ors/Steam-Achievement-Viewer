using Sav.Common.Models;
using Sav.Infrastructure.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SteamAchievementViewer.Services
{
    public interface IGameAchievementsService
    {
        IEnumerable<AchievementComposite> GetAchievementComposites();

        IEnumerable<AchievementComposite> GetRarestAchievements(int page = 1, int count = 100);

        IEnumerable<AchievementComposite> GetLatestAchievements(int page = 1, int count = 100);

        IEnumerable<AchievementComposite> GetClosestAchievements(int page = 1, int count = 100);

        IEnumerable<AchievementComposite> GetGameClosestAchievements(string appid);

        IEnumerable<CompletionGameComposite> GetEasiestGamesToComplete(int page = 1, int count = 100);

        PagedResult<CompletionGameComposite> GetPagedEasiestGamesToComplete(int page = 1, int count = 100);

        Task<PagedResult<CompletionGameComposite>> GetPagedEasiestGamesToCompleteAsync(int page = 1, int count = 100);

        IEnumerable<GameEntity> GetIncompleteGames(int page = 1, int count = 100);

        Task<PagedResult<CompletedGameComposite>> GetPagedCompletedGamesAsync(int page = 1, int count = 25);          

        int GetCompletedAchievementsCount();

        int GetTotalAchievementsCount();
    }
}
