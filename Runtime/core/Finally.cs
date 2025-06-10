#nullable enable
using System;
using JetBrains.Annotations;

namespace Toko.Core
{
    [PublicAPI]
    public struct Finally: IDisposable
    {
        public delegate void OnDispose();
        
        private readonly OnDispose onDispose;
        
        public Finally(OnDispose onDispose) => this.onDispose = onDispose;
        public void Dispose() => onDispose();
    }
}