using NUnit.Framework;
using Toko.Core.Signals;
// ReSharper disable AccessToDisposedClosure

namespace Toko.Tests.Editor.Signals
{
    [TestFixture]
    public class ValueFixture
    {
        [Test]
        public void ValueComputesImmediatelyAfterCreationAndEveryDependencyChange()
        {
            using var a = new Var<int>(2);
            using var b = new Var<int>(6);
            using var rectangleArea = new Val<int>(() => a * b);
            
            Assert.That(rectangleArea.Value, Is.EqualTo(12));
            a.Value = 3;
            Assert.That(rectangleArea.Value, Is.EqualTo(18));
            b.Value = 2;
            Assert.That(rectangleArea.Value, Is.EqualTo(6));
        }

        [Test]
        public void ValueDropsSubscriptionsAfterDispose()
        {
            using var a = new Var<int>(2);
            var a2 = new Val<int>(() => a * 2);
            using var counter = new Counter(a2.AsTrigger());
            
            Assert.That(counter.Value, Is.EqualTo(0));
            a.Value = 3;
            Assert.That(counter.Value, Is.EqualTo(1));
            Assert.That(a2.Value, Is.EqualTo(6));
            
            a2.Dispose();
            a.Value = 1;
            Assert.That(counter.Value, Is.EqualTo(1));
        }

        [Test]
        public void ValueRefreshesDependencyListAfterEveryRecalculation()
        {
            using var a = new Var<int>(1);
            using var b = new Var<int>(2);
            using var cond = new Var<bool>(false);

            using var switched = new Val<int>(() => cond ? a : b);
            using var counter = new Counter(switched.AsTrigger());

            Assert.That(counter.Value, Is.EqualTo(0));
            Assert.That(switched.Value, Is.EqualTo(2));
            a.Value = 3;
            Assert.That(counter.Value, Is.EqualTo(0));
            Assert.That(switched.Value, Is.EqualTo(2));
            b.Value = 4;
            Assert.That(counter.Value, Is.EqualTo(1));
            Assert.That(switched.Value, Is.EqualTo(4));
            cond.Value = true;
            Assert.That(counter.Value, Is.EqualTo(2));
            Assert.That(switched.Value, Is.EqualTo(3));
        }
    }
}