namespace Sav.Common.Interfaces
{
    public interface IListRepository<T>
    {
        void Add(T item);

        void AddRange(IEnumerable<T> items);

        void Clear();

        void RemoveAll(Predicate<T> match);

        IEnumerable<T> GetAll();

        T? Find(Predicate<T> match);
    }
}
