using Sav.Common.Models;
using Sav.Common.Models.DTOs;
using Sav.Infrastructure.Entities;

namespace Sav.Common.Interfaces
{
    public interface IUserEntityRepository : IEntityRepository<UserEntity>
    {
        IEnumerable<UserGameEntity> GetGamesToQueue(string userId, TimeSpan resyncInterval, TimeSpan failedSyncRetryInterval);

        Task<GameSyncStatusPageDto> GetGameSyncStatusAsync(string userId, int page, int count, string? search, string? status);

        void SetGameSyncStatus(string userId, string appId, GameSyncStatus status, string? error);

        void ResetInterruptedSyncs();

        IEnumerable<AchievementComposite> GetUserAchievementComposites(string userId);

        IEnumerable<AchievementComposite> GetUserLatestAchievements(string userId, int page, int count);

        IEnumerable<AchievementComposite> GetUserClosestAchievements(string userId, int page, int count);

        IEnumerable<AchievementComposite> GetUserGameClosestAchievements(string userId, string appId);

        int GetUserCompletedAchievementsCount(string userId);

        IEnumerable<GameEntity> GetUserIncompleteGames(string userId, int take, int skip);

        IEnumerable<AchievementComposite> GetUserRarestAchievements(string userId, int page, int count);

        IEnumerable<CompletionGameComposite> GetUserEasiestGamesToComplete(string userId, int page, int count);

        Task<PagedResult<CompletionGameComposite>> GetPagedUserEasiestGamesToCompleteAsync(string userId, int page, int count);

        Task<PagedResult<CompletedGameComposite>> GetPagedUserCompletedGamesAsync(string userId, int take, int skip);

        int GetUserTotalAchievementsCount(string userId);
    }
}
