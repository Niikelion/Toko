using System.Runtime.CompilerServices;
using JetBrains.Annotations;
// ReSharper disable InconsistentNaming

namespace Toko.Core
{
    [PublicAPI] public sealed class Variable<T>: IObservable<T>
    {
        public event IObservable.UpdateHandler OnUpdate
        {
            add
            {
                onUpdate += value;
                value();
            }
            remove => onUpdate -= value;
        }
        public event IObservable<T>.ChangeHandler OnChange
        {
            add
            {
                onChange += value;
                value(this.value);
            }
            remove => onChange -= value;
        }
        
        private event IObservable.UpdateHandler onUpdate;
        private event IObservable<T>.ChangeHandler onChange;

        public T Value
        {
            get => value;
            set
            {
                this.value = value;
                onUpdate?.Invoke();
                onChange?.Invoke(value);
            }
        }

        private T value;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator T(Variable<T> variable) => variable.value;
        
        public Variable(T value) => this.value = value;
        public void Dispose()
        {
            onUpdate = null;
            onChange = null;
            value = default;
        }
    }
}