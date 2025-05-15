using System;
using JetBrains.Annotations;

namespace Toko.Core
{
    [PublicAPI]
    public struct Finally: IDisposable
    {
        public delegate void OnDispose();
        
        [NotNull] private readonly OnDispose onDispose;
        
        public Finally([NotNull] OnDispose onDispose) => this.onDispose = onDispose;
        public void Dispose() => onDispose();
    }
}