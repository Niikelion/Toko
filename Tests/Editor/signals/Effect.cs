using NUnit.Framework;
using Toko.Core.Signals;
// ReSharper disable AccessToDisposedClosure

namespace Toko.Tests.Editor.Signals
{
    [TestFixture]
    public class EffectFixture
    {
        [Test]
        public void EffectRunsImmediatelyAfterCreationAndEveryDependencyChange()
        {
            using var a = new Var<int>(1);
            int sum = 0;
            using var _ = new Effect(() => sum += a);

            Assert.That(sum, Is.EqualTo(1));
            a.Value = 3;
            Assert.That(sum, Is.EqualTo(4));
            a.Value = 5;
            Assert.That(sum, Is.EqualTo(9));
        }

        [Test]
        public void EffectDropsSubscriptionsAfterDispose()
        {
            using var a = new Var<int>(2);
            int sum = 0;
            var effect = new Effect(() => sum += a * 2);
            using var counter = new Counter(effect);
            
            Assert.That(counter.Value, Is.EqualTo(0));
            Assert.That(sum, Is.EqualTo(4));
            a.Value = 3;
            Assert.That(counter.Value, Is.EqualTo(1));
            Assert.That(sum, Is.EqualTo(10));

            effect.Dispose();
            a.Value = 1;
            Assert.That(counter.Value, Is.EqualTo(1));
            Assert.That(sum, Is.EqualTo(10));
        }

        [Test]
        public void EffectRefreshesDependencyListAfterEveryRecalculation()
        {
            using var a = new Var<int>(1);
            using var b = new Var<int>(2);
            using var cond = new Var<bool>(false);
            
            int sum = 0;
            
            using var effect = new Effect(() => sum += cond ? a : b);
            using var counter = new Counter(effect);

            Assert.That(counter.Value, Is.EqualTo(0));
            Assert.That(sum, Is.EqualTo(2));
            a.Value = 3;
            Assert.That(counter.Value, Is.EqualTo(0));
            Assert.That(sum, Is.EqualTo(2));
            b.Value = 4;
            Assert.That(counter.Value, Is.EqualTo(1));
            Assert.That(sum, Is.EqualTo(6));
            cond.Value = true;
            Assert.That(counter.Value, Is.EqualTo(2));
            Assert.That(sum, Is.EqualTo(9));
        }
    }
}