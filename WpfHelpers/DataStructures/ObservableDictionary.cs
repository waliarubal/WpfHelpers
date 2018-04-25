using NullVoidCreations.WpfHelpers.Base;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NullVoidCreations.WpfHelpers.DataStructures
{
    public class ObservableDictionary<K, V> : NotificationBase, IDictionary<K, V>
    {
        readonly Dictionary<K, V> _dictionary;

        public ObservableDictionary()
        {
            _dictionary = new Dictionary<K, V>();
        }

        #region properties

        public V this[K key]
        {
            get { return _dictionary[key]; }
            set { _dictionary[key] = value; }
        }

        public int Count
        {
            get { return _dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public ICollection<K> Keys
        {
            get { return _dictionary.Keys; }
        }

        public ICollection<V> Values
        {
            get { return _dictionary.Values; }
        }

        #endregion

        public void Add(KeyValuePair<K, V> item)
        {
            Add(item.Key, item.Value);
        }

        public void Add(K key, V value)
        {
            _dictionary.Add(key, value);
            RaisePropertyChanged(nameof(Values));
        }

        public void Clear()
        {
            _dictionary.Clear();
            RaisePropertyChanged(nameof(Values));
        }

        public bool Contains(KeyValuePair<K, V> item)
        {
            return _dictionary.ContainsKey(item.Key) && item.Value.Equals(_dictionary[item.Key]);
        }

        public bool ContainsKey(K key)
        {
            return _dictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        public bool Remove(KeyValuePair<K, V> item)
        {
            return Remove(item.Key);
        }

        public bool Remove(K key)
        {
            var removed = _dictionary.Remove(key);
            if (removed)
                RaisePropertyChanged(nameof(Values));

            return removed;
        }

        public bool TryGetValue(K key, out V value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }
    }
}
