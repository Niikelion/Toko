namespace Toko.Core.Signals
{
    public class Signal<T>: ISignal<T>
    {
        public event ISignal<T>.Handler Event;

        public void Dispose() => Event = null;
        public void Dispatch(T value) => Event?.Invoke(value);
    }
}