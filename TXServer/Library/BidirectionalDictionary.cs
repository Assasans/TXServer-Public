using System.Collections.Generic;

namespace TXServer.Library
{
    /// <summary>
    /// Двунаправленный словарь с поддержкой поиска по значению.
    /// </summary>
    /// <typeparam name="TKey">Тип ключей в словаре.</typeparam>
    /// <typeparam name="TValue">Тип значений в словаре.</typeparam>
    public class BidirectionalDictionary<TKey, TValue>
    {
        private Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
        private Dictionary<TValue, TKey> reverseDictionary = new Dictionary<TValue, TKey>();

        private object writeLock = new object();

        public TValue this[TKey key]
        {
            get
            {
                return dictionary[key];
            }
            set
            {
                lock (writeLock)
                {
                    dictionary[key] = value;

                    reverseDictionary.Remove(value);
                    reverseDictionary[value] = key;
                }
            }
        }

        public TKey this[TValue _value]
        {
            get
            {
                return reverseDictionary[_value];
            }
            set
            {
                lock (writeLock)
                {
                    reverseDictionary[_value] = value;

                    dictionary.Remove(value);
                    dictionary[value] = _value;
                }
            }
        }

        public IEnumerable<TKey> Keys => dictionary.Keys;
        public IEnumerable<TValue> Values => dictionary.Values;
    }
}
