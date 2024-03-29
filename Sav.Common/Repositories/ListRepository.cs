﻿using Sav.Common.Interfaces;

namespace Sav.Common.Repositories
{
    public class ListRepository<T> : IListRepository<T>
    {
        private readonly List<T> _items;

        public ListRepository()
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

        public void AddRange(IEnumerable<T> items)
        {
            _items.AddRange(items);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public void RemoveAll(Predicate<T> match)
        {
            _items.RemoveAll(match);
        }
    }
}
