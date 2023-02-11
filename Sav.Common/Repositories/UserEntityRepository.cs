using Sav.Common.Interfaces;
using Sav.Infrastructure.Entities;

namespace Sav.Common.Repositories
{
    public class UserEntityRepository : EntityRepository<UserEntity>, IUserEntityRepository
    {
        public IEnumerable<UserGameEntity> GetGamesToQueue(string userId, TimeSpan updateInterval)
        {
            var updateTime = DateTime.Now - updateInterval;
            return _context.UserGames.Where(ug => ug.UserId == userId && !string.IsNullOrEmpty(ug.StatsLink) &&
            (!ug.Game.Achievements.Any() || ug.Game.Achievements.Any(a => a.Updated <= updateTime))).AsEnumerable();
        }
    }
}
