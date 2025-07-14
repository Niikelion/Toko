#nullable enable

using System.Collections;
using System.Collections.Generic;

namespace Toko.Core.Signals.Collections
{
    public class ListVar<TValue>: ISignal<IList<TValue>>, IDependableSignal, IList<TValue>
    {
        public event ISignal<IList<TValue>>.Handler? Event;
        
        public int Count => values.Count;
        
        private readonly IList<TValue> values = new List<TValue>();
        private ISignal? triggerWrapper;

        public TValue this[int index]
        {
            get => WithUse(values[index]);
            set
            {
                values[index] = value;
                Update();
            }
        }

        // ReSharper disable once GenericEnumeratorNotDisposed
        public IEnumerator<TValue> GetEnumerator() => WithUse(values.GetEnumerator());

        public bool Contains(TValue item) => WithUse(values.Contains(item));
        public int IndexOf(TValue item) => WithUse(values.IndexOf(item));
        
        public void Dispose()
        {
            Event = null;
            Clear();
        }

        public bool Remove(TValue item)
        {
            bool ret = WithUse(values.Remove(item));
            Update();
            return ret;
        }

        public void RemoveAt(int index)
        {
            values.RemoveAt(index);
            Update();
        }

        public void Add(TValue item)
        {
            values.Add(item);
            Update();
        }

        public void Insert(int index, TValue item)
        {
            values.Insert(index, item);
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
        
        bool ICollection<TValue>.IsReadOnly => values.IsReadOnly;
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        void ICollection<TValue>.CopyTo(TValue[] array, int arrayIndex) => values.CopyTo(array, arrayIndex);
    }
}