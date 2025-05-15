using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Toko.Core
{
    [PublicAPI] public interface IObservable: IDisposable
    {
        delegate void UpdateHandler();
        event UpdateHandler OnUpdate;
    }

    [PublicAPI] public interface IObservable<T> : IObservable
    {
        delegate void ChangeHandler(T value);
        event ChangeHandler OnChange;
    }

    [PublicAPI] public sealed class Observable : IObservable
    {
        [NotNull] public static Observable Trigger => new();
        [NotNull] public static Observable<T> Signal<T>() => new();
        
        public event IObservable.UpdateHandler OnUpdate;
        
        public void Update() => OnUpdate?.Invoke();
        public void Dispose() => OnUpdate = null;
    }

    [PublicAPI] public sealed class Observable<T> : IObservable<T>
    {
        public event IObservable.UpdateHandler OnUpdate;
        public event IObservable<T>.ChangeHandler OnChange;
        
        public void Update(T value)
        {
            OnUpdate?.Invoke();
            OnChange?.Invoke(value);
        }
        public void Dispose()
        {
            OnUpdate = null;
            OnChange = null;
        }
    }
    
    [PublicAPI] public static class ObservableExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Subscribe<T>([NotNull] this T observable, [NotNull] IObservable.UpdateHandler onUpdate) where T: IObservable
        {
            observable.OnUpdate += onUpdate;
            return observable;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IObservable<T> Subscribe<T>(this IObservable<T> observable, IObservable<T>.ChangeHandler onChange)
        {
            observable.OnChange += onChange;
            return observable;
        }
    }
}