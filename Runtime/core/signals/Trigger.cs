#nullable enable

namespace Toko.Core.Signals
{
    public sealed class Trigger: ISignal
    {
        internal abstract class TriggerWrapper : ISignal
        {
            public event ISignal.Handler? Event
            {
                add
                {
                    if (InternalEvent == null) Subscribe();
                    InternalEvent += value;
                }
                remove
                {
                    InternalEvent -= value;
                    if (InternalEvent == null) Unsubscribe();
                }
            }
            private event ISignal.Handler? InternalEvent;

            protected void Dispatch() => InternalEvent?.Invoke();

            public void Dispose()
            {
                if (InternalEvent != null) Unsubscribe();
                InternalEvent = null;
            }

            protected abstract void Subscribe();
            protected abstract void Unsubscribe();
        }

        internal sealed class Wrapper<T> : TriggerWrapper
        {
            private readonly ISignal<T> target;
            
            public Wrapper(ISignal<T> target) => this.target = target;

            protected override void Subscribe() => target.Subscribe(Dispatch);
            protected override void Unsubscribe() => target.Unsubscribe(Dispatch);

            private void Dispatch(T _) => Dispatch();
        }
        
        public event ISignal.Handler? Event;

        public void Dispatch() => Event?.Invoke();
        public void Dispose() => Event = null;
    }
    
    public static class TriggerExtensions
    {
        public static ISignal AsTrigger<T>(this ISignal<T> signal) => new Trigger.Wrapper<T>(signal);
    }
}