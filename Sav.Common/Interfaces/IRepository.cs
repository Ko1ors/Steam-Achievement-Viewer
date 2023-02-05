namespace Sav.Common.Interfaces
{
    public interface IRepository<T>
    {
        void Add(T item);

        IEnumerable<T> GetAll();

        T? Find(Predicate<T> match);
    }
}
