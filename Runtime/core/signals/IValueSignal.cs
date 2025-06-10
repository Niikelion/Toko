#nullable enable
using JetBrains.Annotations;

namespace Toko.Core.Signals
{
    [PublicAPI]
    public interface IValueSignal<T> : ISignal<T>, IDependableSignal
    {
        T Value { get; }
    }
}