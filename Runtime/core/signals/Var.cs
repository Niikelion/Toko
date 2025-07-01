#nullable enable
using System.Runtime.CompilerServices;

namespace Toko.Core.Signals
{
    public sealed class Var<T> : IValueSignal<T>
    {
        public event ISignal<T>.Handler? Event;

        public T Value
        {
            get
            {
                IDependableSignal.RegisterUse(triggerWrapper ??= this.AsTrigger());
                return value;
            }
            set
            {
                this.value = value;
                Event?.Invoke(value);
            }
        }

        private T value;
        private ISignal? triggerWrapper;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator T(Var<T> var) => var.Value;

        public Var(T value) => this.value = value;

        public void Dispose()
        {
            Event = null;
            triggerWrapper?.Dispose();
            value = default!;
        }
    }
#if UNITY_6000_0_OR_NEWER
    public static class VarExtensions
    {
        public static Var<T> Var<T>(this MonoBehaviourWithResources obj, T value)
        {
            var v = new Var<T>(value);
            obj.Use(v);
            return v;
        }
    }
#endif
}