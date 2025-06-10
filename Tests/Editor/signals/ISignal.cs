using NUnit.Framework;
using Toko.Core.Signals;

namespace Toko.Tests.Editor.Signals
{
    [TestFixture]
    public class SignalInterfaceFixture
    {
        [Test]
        public void NonGenericSubscriptionHelpersWorkTheSameAsEvents()
        {
            using var trigger = new Trigger();

            int counter = 0;

            trigger.Event += Increment;
            trigger.Dispatch();
            trigger.Unsubscribe(Increment);
            trigger.Dispatch();
            
            Assert.That(counter, Is.EqualTo(1));
            
            trigger.Subscribe(Increment);
            trigger.Dispatch();
            trigger.Event -= Increment;
            trigger.Dispatch();
            
            Assert.That(counter, Is.EqualTo(2));
            
            return;

            void Increment() => counter++;
        }

        [Test]
        public void GenericSubscriptionHelpersWorkTheSameAsEvents()
        {
            using var signal = new Signal<int>();
            int counter = 0;

            signal.Event += Increment;
            signal.Dispatch(1);
            signal.Unsubscribe(Increment);
            signal.Dispatch(-1);
            Assert.That(counter, Is.EqualTo(1));
            signal.Subscribe(Increment);
            signal.Dispatch(-2);
            signal.Event -= Increment;
            signal.Dispatch(1);
            Assert.That(counter, Is.EqualTo(-1));
            
            return;
            
            void Increment(int number) => counter += number;
        }
    }
}