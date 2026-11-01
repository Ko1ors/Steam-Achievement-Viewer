using Sav.Common.Interfaces;
using Sav.Common.Logs;
using System.Collections.Concurrent;

namespace Sav.Common.Services
{
    public class QueueService<T> : IQueueService<T> where T : class
    {
        private readonly ConcurrentQueue<T> _queue;
        private readonly ConcurrentDictionary<string, byte> _queuedKeys;
        private readonly Func<T, string>? _keySelector;

        public int Size => _queue.Count;

        public QueueService() : this(null)
        {
        }

        /// <param name="keySelector">
        /// Identifies an item so that an item which is already queued is not added again.
        /// Without a key selector every item is accepted.
        /// </param>
        public QueueService(Func<T, string>? keySelector)
        {
            _queue = new ConcurrentQueue<T>();
            _queuedKeys = new ConcurrentDictionary<string, byte>(StringComparer.Ordinal);
            _keySelector = keySelector;
        }

        public void Add(T item)
        {
            if (item is null)
                return;

            if (!TryReserveKey(item))
            {
                Log.Logger.Information("Item is already in the queue, skipping {Item}", item);
                return;
            }

            Log.Logger.Information("Adding item to queue {Item}", item);
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
            Log.Logger.Information("Getting item from queue {RemoveFromQueue}", removeFromQueue);
            if (!removeFromQueue)
            {
                _queue.TryPeek(out var peeked);
                return peeked;
            }

            if (!_queue.TryDequeue(out var item))
                return null;

            ReleaseKey(item);
            return item;
        }

        public IEnumerable<T?> Get(int count)
        {
            Log.Logger.Information("Getting {Count} items from queue", count);
            for (int i = 0; i < count; i++)
            {
                yield return Get(false);
            }
        }

        public IEnumerable<T?> GetAll()
        {
            Log.Logger.Information("Getting all items from queue");
            for (int i = 0; i < Size; i++)
            {
                yield return Get(false);
            }
        }

        private bool TryReserveKey(T item)
        {
            if (_keySelector is null)
                return true;

            return _queuedKeys.TryAdd(_keySelector(item), 0);
        }

        private void ReleaseKey(T item)
        {
            if (_keySelector is null)
                return;

            _queuedKeys.TryRemove(_keySelector(item), out _);
        }
    }
}
