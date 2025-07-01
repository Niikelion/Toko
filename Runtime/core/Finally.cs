#nullable enable
using System;

namespace Toko.Core
{
    public readonly struct Finally: IDisposable
    {
        public delegate void OnDispose();
        
        private readonly OnDispose onDispose;
        
        public Finally(OnDispose onDispose) => this.onDispose = onDispose;
        public void Dispose() => onDispose();
    }
}