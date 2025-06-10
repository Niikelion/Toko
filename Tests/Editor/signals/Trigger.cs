using NUnit.Framework;
using Toko.Core.Signals;

namespace Toko.Tests.Editor.Signals
{
    [TestFixture]
    public class TriggerFixture
    {
        [Test]
        public void TriggerEventGetsDispatched()
        {
            using var trigger = new Trigger();
            int counter = 0;
            
            trigger.Event += () => counter++;
            
            Assert.That(counter, Is.EqualTo(0));
            trigger.Dispatch();
            Assert.That(counter, Is.EqualTo(1));
            trigger.Dispatch();
            Assert.That(counter, Is.EqualTo(2));
        }

        [Test]
        public void TriggerDropsSubscriptionsAfterDispose()
        {
            var trigger = new Trigger();
            int counter = 0;
            trigger.Event += () => counter++;
            Assert.That(counter, Is.EqualTo(0));
            trigger.Dispatch();
            Assert.That(counter, Is.EqualTo(1));
            trigger.Dispose();
            trigger.Dispatch();
            Assert.That(counter, Is.EqualTo(1));
        }
    }
}