using Sav.Common.Interfaces;
using Sav.Common.Logs;
using Sav.Infrastructure;
using Sav.Infrastructure.Entities;
using System.Linq.Expressions;

namespace Sav.Common.Repositories
{
    public class EntityRepository<TEntity> : IEntityRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly SteamContext _context;

        public EntityRepository()
        {
            _context = new SteamContext();
        }

        public async Task<TEntity?> GetByKeysAsync(params object[] keys)
        {
            Log.Logger.Information("Getting entity {Entity} by keys {Keys}", typeof(TEntity).Name, keys);   
            return await _context.Set<TEntity>().FindAsync(keys);
        }

        public async Task<int> AddAsync(TEntity entity)
        {
            entity.Inserted = DateTime.Now;
            entity.Updated = entity.Inserted;
            await _context.Set<TEntity>().AddAsync(entity);
            return await _context.SaveChangesAsync();
        }

        public Task<int> UpdateAsync(TEntity entity)
        {
            entity.Updated = DateTime.Now;
            _context.Set<TEntity>().Update(entity);
            return _context.SaveChangesAsync();
        }

        public Task<int> DeleteAsync(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
            return _context.SaveChangesAsync();
        }

        public virtual async Task AddOrUpdateAsync(TEntity entity)
        {
            var existingEntity = _context.Set<TEntity>().Find(entity.GetKeys());

            if (existingEntity == null)
            {
                await AddAsync(entity);
            }
            else
            {
                _context.Entry(existingEntity).CurrentValues.SetValues(entity);
                await UpdateAsync(existingEntity);
            }
        }

        public TEntity? GetByKeys(params object[] keys)
        {
            Log.Logger.Information("Getting entity {Entity} by keys {Keys}", typeof(TEntity).Name, keys);
            return _context.Set<TEntity>().Find(keys);
        }

        public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> predicate)
        {
            Log.Logger.Information("Getting entities {Entity} by predicate {Predicate}", typeof(TEntity).Name, predicate);
            return _context.Set<TEntity>().Where(predicate).AsEnumerable();
        }

        public IEnumerable<TEntity> GetAll()
        {
            Log.Logger.Information("Getting all entities {Entity}", typeof(TEntity).Name);
            return _context.Set<TEntity>().ToList();
        }

        public void Add(TEntity entity)
        {
            entity.Inserted = DateTime.Now;
            entity.Updated = entity.Inserted;
            _context.Set<TEntity>().Add(entity);
            _context.SaveChanges();
        }

        public void Update(TEntity entity)
        {
            entity.Updated = DateTime.Now;
            _context.Set<TEntity>().Update(entity);
            _context.SaveChanges();
        }

        public void Delete(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
            _context.SaveChanges();
        }

        public void AddOrUpdate(TEntity entity)
        {
            var existingEntity = _context.Set<TEntity>().Find(entity.GetKeys());

            if (existingEntity == null)
            {
                Add(entity);
            }
            else
            {
                _context.Entry(existingEntity).CurrentValues.SetValues(entity);
                Update(existingEntity);
            }
        }

        public void Refresh()
        {
            Log.Logger.Information("Refreshing context");
            _context.ChangeTracker.Clear();
        }
    }
}
