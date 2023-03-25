using Sav.Common.Interfaces;
using Sav.Infrastructure.Entities;

namespace Sav.Common.Repositories
{
    public class GameEntityRepository : EntityRepository<GameEntity>, IGameEntityRepository
    {
        public override async Task AddOrUpdateAsync(GameEntity entity)
        {
            var existingEntity = _context.Set<GameEntity>().Find(entity.GetKeys());

            if (existingEntity == null)
            {
                await AddAsync(entity);
            }
            else
            {
                entity.GameIcon ??= existingEntity.GameIcon;
                entity.GameLogoSmall ??= existingEntity.GameLogoSmall;

                _context.Entry(existingEntity).CurrentValues.SetValues(entity);
                await UpdateAsync(existingEntity);
            }
        }
    }
}
