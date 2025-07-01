#nullable enable

namespace Toko.Core.Signals
{
    public class Effect : DependentBase, ISignal
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
#if UNITY_6000
    public static class EffectExtensions
    {
        public static Effect MakeEffect(this MonoBehaviourWithResources obj, IDependentOnSignals.Callback callback)
        {
            var effect = new Effect(callback);
            obj.Use(effect);
            return effect;
        }
    }
#endif
}