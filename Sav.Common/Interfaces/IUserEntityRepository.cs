using Sav.Common.Models;
using Sav.Infrastructure.Entities;

namespace Sav.Common.Interfaces
{
    public interface IUserEntityRepository : IEntityRepository<UserEntity>
    {
        IEnumerable<UserGameEntity> GetGamesToQueue(string userId, TimeSpan updateInterval);

        IEnumerable<AchievementComposite> GetUserAchievementComposites(string userId);
    }
}
