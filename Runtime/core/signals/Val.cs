#nullable enable
using System.Runtime.CompilerServices;

namespace Toko.Core.Signals
{
    public sealed class Val<T> : DependentBase, IValueSignal<T>
    {
        public delegate T Factory();

        public event ISignal<T>.Handler? Event;

        public T Value
        {
            get
            {
                IDependableSignal.RegisterUse(triggerWrapper ??= this.AsTrigger());
                return value;
            }
        }

        private T value;
        private ISignal? triggerWrapper;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator T(Val<T> variable) => variable.value;

        public Val(Factory factory)
        {
            value = default!;
            Init(() => value = factory());
        }

        public override void Dispose()
        {
            base.Dispose();
            triggerWrapper?.Dispose();
            value = default!;
        }

        protected override void AfterRun() => Event?.Invoke(value);
    }
#if UNITY_6000_0_OR_NEWER
    public static class ValueExtensions
    {
        public static Val<T> Compute<T>(this MonoBehaviourWithResources obj, Val<T>.Factory factory)
        {
            var val = new Val<T>(factory);
            obj.Use(val);
            return val;
        }
    }
#endif
}