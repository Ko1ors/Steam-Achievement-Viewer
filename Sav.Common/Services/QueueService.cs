using Sav.Common.Interfaces;
using System.Collections.Concurrent;

namespace Sav.Common.Services
{
    public class QueueService<T> : IQueueService<T> where T : class
    {
        private readonly ConcurrentQueue<T> _queue;

        public int Size => _queue.Count;

        public QueueService()
        {
            _queue = new ConcurrentQueue<T>();
        }

        public void Add(T item)
        {
            _queue.Enqueue(item);
        }

        public void Add(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public T? Get(bool removeFromQueue = true)
        {
            T? item;
            if (removeFromQueue)
                _queue.TryDequeue(out item);
            else
                _queue.TryPeek(out item);
            return item;
        }

        public IEnumerable<T?> Get(int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return Get(false);
            }
        }

        public IEnumerable<T?> GetAll()
        {
            for (int i = 0; i < Size; i++)
            {
                yield return Get(false);
            }
        }
    }
}
