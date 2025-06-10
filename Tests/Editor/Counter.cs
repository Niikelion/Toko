using System;
using Toko.Core.Signals;

namespace Toko.Tests.Editor
{
    public class Counter: IDisposable
    {
        public int Value { get; private set; }
        private readonly ISignal source;

        public Counter(ISignal source)
        {
            this.source = source;
            source.Event += Increment;
        }

        public void Dispose() => source.Event -= Increment;
        private void Increment() => Value++;
    }
}