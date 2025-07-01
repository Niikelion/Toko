#nullable enable
using System;

namespace Toko.Core.Signals
{
    public interface ISignal: IDisposable
    {
        delegate void Handler();
        event Handler? Event;
    }

    public interface ISignal<T>: IDisposable
    {
        delegate void Handler(T value);
        event Handler? Event;
    }

    public static class SignalSubscriptionExtensions
    {
        public static void Subscribe(this ISignal signal, ISignal.Handler handler) => signal.Event += handler;
        public static void Unsubscribe(this ISignal signal, ISignal.Handler handler) => signal.Event -= handler;
        
        public static void Subscribe<T>(this ISignal<T> signal, ISignal<T>.Handler handler) => signal.Event += handler;
        public static void Unsubscribe<T>(this ISignal<T> signal, ISignal<T>.Handler handler) => signal.Event -= handler;
    }
}