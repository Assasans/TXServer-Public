using System;
using System.Collections;
using System.Collections.Generic;

namespace TXServer.Database.Observable
{
    public class ChangedEventArgs
    {
    }

    public class ObservableList<T> : IList<T>
    {
        private List<T> _list = new List<T>();

        public event EventHandler<ChangedEventArgs> Changed;

        public void Add(T item)
        {
            _list.Add(item);
            Changed?.Invoke(this, new ChangedEventArgs());
        }

        public T this[int index]
        {
            get => _list[index];
            set
            {
                _list[index] = value;
                Changed?.Invoke(this, new ChangedEventArgs());
            }
        }
        public int Count => _list.Count;
        public bool IsReadOnly => false;
        public void Clear()
        {
            _list.Clear();
            Changed?.Invoke(this, new ChangedEventArgs());
        }
        public bool Contains(T item) => _list.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
        public int IndexOf(T item) => _list.IndexOf(item);
        public void Insert(int index, T item)
        {
            _list.Insert(index, item);
            Changed?.Invoke(this, new ChangedEventArgs());
        }
        public bool Remove(T item) => _list.Remove(item);
        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
            Changed?.Invoke(this, new ChangedEventArgs());
        }
        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
    }

    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

        public event EventHandler<ChangedEventArgs> Changed;

        public void Add(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
            Changed?.Invoke(this, new ChangedEventArgs());
        }

        public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);
        public TValue this[TKey key]
        {
            get => _dictionary[key];
            set
            {
                _dictionary[key] = value;
                Changed?.Invoke(this, new ChangedEventArgs());
            }
        }
        public ICollection<TKey> Keys => _dictionary.Keys;
        public ICollection<TValue> Values => _dictionary.Values;
        public int Count => _dictionary.Count;
        public bool IsReadOnly => false;
        public void Add(KeyValuePair<TKey, TValue> item) => _dictionary.Add(item.Key, item.Value);
        public void Clear()
        {
            _dictionary.Clear();
            Changed?.Invoke(this, new ChangedEventArgs());
        }
        public bool Contains(KeyValuePair<TKey, TValue> item) => ContainsKey(item.Key);
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((ICollection<KeyValuePair<TKey, TValue>>) _dictionary).CopyTo(array, arrayIndex);
        public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);
        public bool ContainsValue(TValue value) => _dictionary.ContainsValue(value);
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();
        public bool Remove(TKey key)
        {
            if (!_dictionary.Remove(key)) return false;
            Changed?.Invoke(this, new ChangedEventArgs());
            return true;
        }
        public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);
        public bool TryAdd(TKey key, TValue value)
        {
            if (!_dictionary.TryAdd(key, value)) return false;
            Changed?.Invoke(this, new ChangedEventArgs());
            return true;
        }
        IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();
    }
}
