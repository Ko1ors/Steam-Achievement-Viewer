using Sav.Infrastructure.Entities;

namespace Sav.Common.Interfaces
{
    public interface IUserEntityRepository : IEntityRepository<UserEntity>
    {
        IEnumerable<UserGameEntity> GetGamesToQueue(string userId, TimeSpan updateInterval);
    }
}
