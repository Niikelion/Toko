#nullable enable

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Toko.Core.Signals.Collections
{
    public class DictionaryVar<TKey, TValue>: ISignal<IDictionary<TKey, TValue>>, IDependableSignal, IDictionary<TKey, TValue>
    {
        public event ISignal<IDictionary<TKey, TValue>>.Handler? Event;

        public int Count => WithUse(values.Count);
        public Dictionary<TKey, TValue>.KeyCollection Keys => WithUse(values.Keys);
        public Dictionary<TKey, TValue>.ValueCollection Values => WithUse(values.Values);
        
        private readonly Dictionary<TKey, TValue> values = new();
        private ISignal? triggerWrapper;
        
        public TValue this[TKey key]
        {
            get => WithUse(values[key]);
            set
            {
                values[key] = value;
                Update();
            }
        }
        
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => WithUse(values.GetEnumerator());
        
        public bool Contains(KeyValuePair<TKey, TValue> item) => WithUse(values.Contains(item));
        public bool ContainsKey(TKey key) => WithUse(values.ContainsKey(key));
        public bool TryGetValue(TKey key, out TValue value) => WithUse(values.TryGetValue(key, out value));
        
        public void Dispose()
        {
            Event = null;
            triggerWrapper?.Dispose();
            Clear();
        }

        public bool Remove(TKey key)
        {
            bool ret = WithUse(values.Remove(key));
            Update();
            return ret;
        }

        public void Add(TKey key, TValue value)
        {
            values.Add(key, value);
            Update();
        }
        
        public void Clear()
        {
            values.Clear();
            Update();
        }

        private void Update() => Event?.Invoke(this);

        private T WithUse<T>(T value)
        {
            RegisterUse();
            return value;
        }
        private void RegisterUse() => IDependableSignal.RegisterUse(triggerWrapper ??= this.AsTrigger());
        
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => WithUse(((IDictionary)values).IsReadOnly);
        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;
        ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            RegisterUse();
            ((IDictionary)values).CopyTo(array, arrayIndex);
        }
    }
}