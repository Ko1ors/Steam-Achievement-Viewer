using Sav.Common.Models;
using Sav.Infrastructure.Entities;

namespace Sav.Common.Interfaces
{
    public interface IUserEntityRepository : IEntityRepository<UserEntity>
    {
        IEnumerable<UserGameEntity> GetGamesToQueue(string userId);

        IEnumerable<UserGameEntity> GetRecentGamesToQueue(string userId, TimeSpan updateInterval);

        IEnumerable<AchievementComposite> GetUserAchievementComposites(string userId);

        IEnumerable<AchievementComposite> GetUserLatestAchievements(string userId, int page, int count);

        IEnumerable<AchievementComposite>  GetUserClosestAchievements(string userId, int page, int count);

        IEnumerable<AchievementComposite> GetUserGameClosestAchievements(string userId, string appId);

        int GetUserCompletedAchievementsCount(string userId);

        IEnumerable<GameEntity> GetUserIncompleteGames(string userId, int page, int count);

        IEnumerable<AchievementComposite> GetUserRarestAchievements(string userId, int page, int count);

        IEnumerable<CompletionGameComposite> GetUserEasiestGamesToComplete(string userId, int page, int count);

        PagedResult<CompletionGameComposite> GetPagedUserEasiestGamesToComplete(string userId, int page, int count);


        int GetUserTotalAchievementsCount(string userId);
    }
}
