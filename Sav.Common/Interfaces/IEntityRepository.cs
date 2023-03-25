using System.Linq.Expressions;

namespace Sav.Common.Interfaces
{
    public interface IEntityRepository<TEntity> where TEntity : class
    {
        Task<TEntity?> GetByKeysAsync(params object[] keys);

        Task<int> AddAsync(TEntity entity);

        Task<int> UpdateAsync(TEntity entity);

        Task<int> DeleteAsync(TEntity entity);

        Task AddOrUpdateAsync(TEntity entity);

        TEntity? GetByKeys(params object[] keys);

        IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> predicate);

        IEnumerable<TEntity> GetAll();

        void Add(TEntity entity);

        void Update(TEntity entity);

        void Delete(TEntity entity);

        void AddOrUpdate(TEntity entity);

        void Refresh();
    }
}
