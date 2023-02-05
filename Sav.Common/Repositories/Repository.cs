using Sav.Common.Interfaces;

namespace Sav.Common.Repositories
{
    public class Repository<T> : IRepository<T>
    {
        private readonly List<T> _items;

        public Repository()
        {
            _items = new List<T>();
        }

        public void Add(T item)
        {
            _items.Add(item);
        }

        public IEnumerable<T> GetAll()
        {
            return _items;
        }

        public T? Find(Predicate<T> match)
        {
            return _items.Find(match);
        }
    }
}
