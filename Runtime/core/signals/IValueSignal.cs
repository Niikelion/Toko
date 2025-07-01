#nullable enable

namespace Toko.Core.Signals
{
    public interface IValueSignal<T> : ISignal<T>, IDependableSignal
    {
        T Value { get; }
    }
}