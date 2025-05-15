using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
// ReSharper disable InconsistentNaming

namespace Toko.Core
{
    [PublicAPI] public sealed class Computation<T>: IObservable<T>
    {
        public delegate T ValueFactory();
        public delegate bool ComparisonPredicate(T oldValue, T newValue);
        
        public event IObservable.UpdateHandler OnUpdate
        {
            add
            {
                if (onUpdate == null && onChange == null) SubscribeToDependencies();
                onUpdate += value;
            }
            remove
            {
                onUpdate -= value;
                if (onUpdate == null && onChange == null) UnsubscribeFromDependencies();
            }
        }

        public event IObservable<T>.ChangeHandler OnChange
        {
            add
            {
                if (onUpdate == null && onChange == null) SubscribeToDependencies();
                onChange += value;
                value(Value);
            }
            remove
            {
                onChange -= value;
                if (onUpdate == null && onChange == null) UnsubscribeFromDependencies();
            }
        }
        
        private event IObservable.UpdateHandler onUpdate;
        private event IObservable<T>.ChangeHandler onChange;

        public T Value { get; private set; }

        [NotNull] private ValueFactory factory;
        [NotNull] private ComparisonPredicate comparison;
        [NotNull] private IObservable[] dependencies;

        public static implicit operator T(Computation<T> v) => v.Value;

        public Computation([NotNull] ValueFactory factory, [NotNull] ComparisonPredicate comparison, params IObservable[] dependencies)
        {
            this.comparison = comparison;
            this.dependencies = dependencies;
            this.factory = factory;
            Value = factory();
        }

        public Computation([NotNull] ValueFactory factory, params IObservable[] dependencies):
            this(factory, comparison: EqualityComparer<T>.Default.Equals, dependencies: dependencies) {}
        
        public void Dispose()
        {
            UnsubscribeFromDependencies();
            
            onUpdate = null;
            onChange = null;
            Value = default;
        }

        private void RecalculateAndCache()
        {
            var previousValue = Value;
            Value = factory();

            if (comparison(previousValue, Value)) return;
            
            onUpdate?.Invoke();
            onChange?.Invoke(Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SubscribeToDependencies()
        {
            foreach (var dependency in dependencies) dependency.OnUpdate += RecalculateAndCache;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UnsubscribeFromDependencies()
        {
            foreach (var dependency in dependencies) dependency.OnUpdate -= RecalculateAndCache;
        }
    }
}