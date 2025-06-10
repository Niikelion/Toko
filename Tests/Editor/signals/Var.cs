using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Toko.Core.Signals;

namespace Toko.Tests.Editor.Signals
{
    [TestFixture]
    public class VarFixture: IDependentOnSignals
    {
        [Test]
        public void VarFiresEventOnEveryAssignment()
        {
            using var variable = new Var<int>(0);

            int counter = 0;

            variable.Event += Increment;
            variable.Value = 1;
            Assert.That(counter, Is.EqualTo(1));
            variable.Value = 1;
            Assert.That(counter, Is.EqualTo(2));
            variable.Value = -3;
            Assert.That(counter, Is.EqualTo(-1));
            
            return;

            void Increment(int number) => counter += number;
        }

        [Test]
        public void VarDisposeDropsSubscribers()
        {
            var variable = new Var<int>(0);
            int counter = 0;

            variable.Event += Increment;
            variable.Value = 1;
            Assert.That(counter, Is.EqualTo(1));
            variable.Dispose();
            variable.Value = 2;
            Assert.That(counter, Is.EqualTo(1));
            
            return;
            
            void Increment(int number) => counter += number;
        }

        [Test]
        [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
        public void VarValueAccessIsTrackedWithoutDuplicates()
        {
            using var variable = new Var<int>(0);
            var uses = IDependentOnSignals.RunCatchingUses(() =>
            {
                variable.Value += 1;
                variable.Value *= 2;
            });
            Assert.That(variable.Value, Is.EqualTo(2));
            Assert.That(uses, Is.Not.Null);
            Assert.That(uses.Length, Is.EqualTo(1));

            int counter = 0;

            uses[0].Subscribe(Increment);
            variable.Value = 1;
            Assert.That(counter, Is.EqualTo(1));
            
            return;
            
            void Increment() => counter ++;
        }
    }
}