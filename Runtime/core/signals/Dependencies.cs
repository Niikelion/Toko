#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Toko.Core.Signals
{
    [PublicAPI]
    public interface IDependentOnSignals
    {
        public delegate void Callback();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static ISignal[] RunCatchingUses(Callback callback)
        {
            var uses = new HashSet<ISignal>();
            using (IDependableSignal.TrackingContext.Provide(uses)) callback();
            return uses.ToArray();
        }
    }

    [PublicAPI]
    public interface IDependableSignal
    {
        internal static Context<HashSet<ISignal>?> TrackingContext = new (null);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static void RegisterUse(ISignal value) => TrackingContext.Value?.Add(value);
    }

    [PublicAPI]
    public abstract class DependentBase : IDependentOnSignals, IDisposable
    {
        private ISignal[] dependencies = Array.Empty<ISignal>();
        private IDependentOnSignals.Callback callback = Noop;

        public virtual void Dispose() => UnsubscribeFromDependencies();

        protected void Init(IDependentOnSignals.Callback callback)
        {
            this.callback = callback;
            Run();
        }

        protected virtual void AfterRun() { }

        private static void Noop() { }

        private void Run()
        {
            UnsubscribeFromDependencies();
            dependencies = IDependentOnSignals.RunCatchingUses(callback);
            SubscribeToDependencies();
            AfterRun();
        }
        private void UnsubscribeFromDependencies()
        {
            foreach (var signal in dependencies) signal.Unsubscribe(Run);
        }

        private void SubscribeToDependencies()
        {
            foreach (var signal in dependencies) signal.Subscribe(Run);
        }
    }
}