using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
// ReSharper disable InconsistentNaming

namespace Toko.Core
{
    [PublicAPI] public interface IValue<T> : IObservable<T>
    {
        public T Value { get; }

        private static Context<HashSet<IValue<T>>> TrackingContext = new (null);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static void RegisterUse(IValue<T> value) => TrackingContext.Value?.Add(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static IValue<T>[] RunCatchingUses(Action action)
        {
            var uses = new HashSet<IValue<T>>();
            using (TrackingContext.Provide(uses)) action();

            return uses.ToArray();
        }
    }
    
    [PublicAPI] public sealed class Variable<T>: IValue<T>
    {
        public event IObservable.UpdateHandler OnUpdate;
        public event IObservable<T>.ChangeHandler OnChange;

        public T Value
        {
            get
            {
                IValue<T>.RegisterUse(this);
                return value;
            }
            set
            {
                this.value = value;
                OnUpdate?.Invoke();
                OnChange?.Invoke(value);
            }
        }

        private T value;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator T(Variable<T> variable) => variable.value;
        
        public Variable(T value) => this.value = value;
        public void Dispose()
        {
            OnUpdate = null;
            OnChange = null;
            value = default;
        }
    }
}