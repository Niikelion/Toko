#nullable enable
using JetBrains.Annotations;

namespace Toko.Core.Signals
{
    [PublicAPI]
    public class Effect: DependentBase, ISignal
    {
        public event ISignal.Handler? Event;

        public Effect(IDependentOnSignals.Callback callback) => Init(callback);

        public override void Dispose()
        {
            base.Dispose();
            Event = null;
        }

        protected override void AfterRun() => Event?.Invoke();
    }

    [PublicAPI]
    public static class EffectExtensions
    {
        public static Effect MakeEffect(this MonoBehaviourWithResources obj, IDependentOnSignals.Callback callback)
        {
            var effect = new Effect(callback);
            obj.Use(effect);
            return effect;
        }
    }
}