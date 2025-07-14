#nullable enable

using System.Collections;
using System.Collections.Generic;

namespace Toko.Core.Signals.Collections
{
    public class HashSetVar<TValue>: ISignal<ISet<TValue>>, IDependableSignal, ISet<TValue>
    {
        public event ISignal<ISet<TValue>>.Handler? Event;

        public int Count => WithUse(values.Count);
        
        private readonly ISet<TValue> values = new HashSet<TValue>();
        private ISignal? triggerWrapper;

        // ReSharper disable once GenericEnumeratorNotDisposed
        public IEnumerator<TValue> GetEnumerator() => WithUse(values.GetEnumerator());
        
        public bool Contains(TValue item) => WithUse(values.Contains(item));
        public bool SetEquals(IEnumerable<TValue> other) => WithUse(values.SetEquals(other));
        public bool IsProperSubsetOf(IEnumerable<TValue> other) => WithUse(values.IsProperSubsetOf(other));
        public bool IsProperSupersetOf(IEnumerable<TValue> other) => WithUse(values.IsProperSupersetOf(other));
        public bool IsSubsetOf(IEnumerable<TValue> other) => WithUse(values.IsSubsetOf(other));
        public bool IsSupersetOf(IEnumerable<TValue> other) => WithUse(values.IsSupersetOf(other));
        public bool Overlaps(IEnumerable<TValue> other) => WithUse(values.Overlaps(other));

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

        public bool Add(TValue item)
        {
            bool ret = WithUse(values.Add(item));
            Update();
            return ret;
        }

        public void Clear()
        {
            values.Clear();
            Update();
        }

        public void ExceptWith(IEnumerable<TValue> other)
        {
            values.ExceptWith(other);
            Update();
        }

        public void IntersectWith(IEnumerable<TValue> other)
        {
            values.IntersectWith(other);
            Update();
        }

        public void SymmetricExceptWith(IEnumerable<TValue> other)
        {
            values.SymmetricExceptWith(other);
            Update();
        }

        public void UnionWith(IEnumerable<TValue> other)
        {
            values.UnionWith(other);
            Update();
        }

        private void Update() => Event?.Invoke(this);

        private T WithUse<T>(T value)
        {
            RegisterUse();
            return value;
        }
        private void RegisterUse() => IDependableSignal.RegisterUse(triggerWrapper ??= this.AsTrigger());
        
        bool ICollection<TValue>.IsReadOnly => WithUse(values.IsReadOnly);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        void ICollection<TValue>.Add(TValue item) => Add(item);
        void ICollection<TValue>.CopyTo(TValue[] array, int arrayIndex) => values.CopyTo(array, arrayIndex);
    }
}