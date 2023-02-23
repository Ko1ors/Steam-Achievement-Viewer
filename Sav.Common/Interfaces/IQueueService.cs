namespace Sav.Common.Interfaces
{
    public interface IQueueService<T> where T : class
    {
        int Size { get; }

        void Add(T item);

        void Add(IEnumerable<T> items);

        T? Get(bool removeFromQueue = true);

        IEnumerable<T?> Get(int count);

        IEnumerable<T?> GetAll();
    }
}
