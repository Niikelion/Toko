using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Toko.Core
{
    [PublicAPI] public sealed class Computation<T>: IValue<T>
    {
        public delegate T ValueFactory();
        public delegate bool ComparisonPredicate(T oldValue, T newValue);

        public event IObservable.UpdateHandler OnUpdate;
        public event IObservable<T>.ChangeHandler OnChange;

        public T Value
        {
            get
            {
                IValue<T>.RegisterUse(this);
                return value;
            }
            private set
            {
                var previous = this.value;
                this.value = value;
                
                if (comparison(previous, this.value)) return;
                
                OnUpdate?.Invoke();
                OnChange?.Invoke(value);
            }
        }

        private T value;

        [NotNull] private ValueFactory factory;
        [NotNull] private ComparisonPredicate comparison;
        [NotNull] private IValue<T>[] dependencies;

        public static implicit operator T(Computation<T> v) => v.Value;

        public Computation([NotNull] ValueFactory factory, [NotNull] ComparisonPredicate comparison)
        {
            this.comparison = comparison;
            this.factory = factory;
            dependencies = Array.Empty<IValue<T>>();
            RecalculateAndCache(true);
        }

        public Computation([NotNull] ValueFactory factory): this(factory, comparison: EqualityComparer<T>.Default.Equals) {}
        
        public void Dispose()
        {
            UnsubscribeFromDependencies();
            
            OnUpdate = null;
            OnChange = null;
            Value = default;
        }

        private void RecalculateAndCache(bool force = false)
        {
            UnsubscribeFromDependencies();
            dependencies = IValue<T>.RunCatchingUses(() => Value = factory());
        }
        private void OnDependenciesChanged() => RecalculateAndCache();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SubscribeToDependencies()
        {
            foreach (var dependency in dependencies) dependency.OnUpdate += OnDependenciesChanged;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UnsubscribeFromDependencies()
        {
            foreach (var dependency in dependencies) dependency.OnUpdate -= OnDependenciesChanged;
        }
    }
}