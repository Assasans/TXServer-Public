using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace TXServer.Library
{
    public class ConcurrentHashSet<T> : ISet<T>
    {
        private readonly ConcurrentDictionary<int, T> dictionary = new ConcurrentDictionary<int, T>();

        public int Count => dictionary.Count;

        public bool IsReadOnly => false;

        public bool Add(T item) => dictionary.TryAdd(item.GetHashCode(), item);

        public void Clear() => dictionary.Clear();

        public bool Contains(T item) => dictionary.ContainsKey(item.GetHashCode());

        public void CopyTo(T[] array, int arrayIndex) => dictionary.Values.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() => dictionary.Values.GetEnumerator();

        public bool TryGetValue(T equalValue, out T actualValue) => dictionary.TryGetValue(equalValue.GetHashCode(), out actualValue);

        public bool Remove(T item) => dictionary.TryRemove(item.GetHashCode(), out _);

        void ICollection<T>.Add(T item) => dictionary.TryAdd(item.GetHashCode(), item);

        IEnumerator IEnumerable.GetEnumerator() => dictionary.Values.GetEnumerator();


        public void ExceptWith(IEnumerable<T> other) => throw new System.NotImplementedException();

        public void IntersectWith(IEnumerable<T> other) => throw new System.NotImplementedException();

        public bool IsProperSubsetOf(IEnumerable<T> other) => throw new System.NotImplementedException();

        public bool IsProperSupersetOf(IEnumerable<T> other) => throw new System.NotImplementedException();

        public bool IsSubsetOf(IEnumerable<T> other) => throw new System.NotImplementedException();

        public bool IsSupersetOf(IEnumerable<T> other) => throw new System.NotImplementedException();

        public bool Overlaps(IEnumerable<T> other) => throw new System.NotImplementedException();

        public bool SetEquals(IEnumerable<T> other) => throw new System.NotSupportedException();

        public void SymmetricExceptWith(IEnumerable<T> other) => throw new System.NotImplementedException();

        public void UnionWith(IEnumerable<T> other) => throw new System.NotImplementedException();

        
    }
}
