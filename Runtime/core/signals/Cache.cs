#nullable enable

using System;
using System.Collections.Generic;

namespace Toko.Core.Signals
{
    public sealed class Cache<T>: IValueSignal<T>
    {
        public event ISignal<T>.Handler? Event;

        public T Value
        {
            get
            {
                IDependableSignal.RegisterUse(triggerWrapper ??= this.AsTrigger());
                return value;
            }
            private set
            {
                if (comparator(this.value, value)) return;
                
                this.value = value;
                Event?.Invoke(value);
            }
        }

        private T value;
        private ISignal? triggerWrapper;
        private readonly ISignal<T> source;
        private readonly Func<T, T, bool> comparator;

        public Cache(IValueSignal<T> source, Func<T, T, bool>? comparator = null)
        {
            value = source.Value;
            this.source = source;
            source.Event += OnSourceUpdate;
            this.comparator = comparator ?? EqualityComparer<T>.Default.Equals;
        }

        public Cache(ISignal<T> source, T value, Func<T, T, bool>? comparator = null)
        {
            this.value = value;
            this.source = source;
            source.Event += OnSourceUpdate;
            this.comparator = comparator ?? EqualityComparer<T>.Default.Equals;
        }
        
        public void Dispose()
        {
            Event = null;
            triggerWrapper?.Dispose();
            value = default!;
            source.Event -= OnSourceUpdate;
        }

        private void OnSourceUpdate(T value) => Value = value;
    }

    public static class CacheExtensions
    {
        public static Cache<T> Cache<T>(this IValueSignal<T> signal, Func<T, T, bool>? comparator = null) =>
            new(signal, comparator);
        public static Cache<T> Cache<T>(this ISignal<T> target, T initialValue, Func<T, T, bool>? comparator = null) =>
            new(target, initialValue, comparator);
    }
}