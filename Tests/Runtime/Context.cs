using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Toko.Core;

namespace Toko.Tests.Runtime
{
    [TestFixture]
    public class Context
    {
        [Test]
        public void TheProvidedValueDoesNotLeakToTheParentScope()
        {
            var context = new Context<int>(5);
            
            Assert.That(context.Value, Is.EqualTo(5));
            using (context.Provide(6))
            {
                Assert.That(context.Value, Is.EqualTo(6));
            }
            Assert.That(context.Value, Is.EqualTo(5));
        }

        [Test]
        public void MultipleContextsWithTheSameTypeWorkInTheSameScope()
        {
            var context1 = new Context<int>(1);
            var context2 = new Context<int>(2);

            using (context1.Provide(3))
            using (context2.Provide(4))
            {
                Assert.That(context1.Value, Is.EqualTo(3));
                Assert.That(context2.Value, Is.EqualTo(4));
            }
        }

        [Test]
        public void ContextPreservesValueDownTheExecutionTree()
        {
            var context = new Context<int>(5);

            Foo();
            return;

            void Foo()
            {
                Bar(5);
                using (context.Provide(6)) Bar(6);
                Bar(5);
                using (context.Provide(7)) Bar(7);
            }

            void Bar(int n) => Assert.That(context.Value, Is.EqualTo(n));
        }

        [Test]
        public async Task ContextPreservesValueInAsync()
        {
            var context = new Context<int>(0);
            
            Assert.That(context.Value, Is.EqualTo(0));
            Task task;
            
            using (context.Provide(1))
            {
                task = Foo();
            }
            
            Assert.That(context.Value, Is.EqualTo(0));
            await task;
            Assert.That(context.Value, Is.EqualTo(0));
            return;

            async Task Foo()
            {
                Assert.That(context.Value, Is.EqualTo(1));
                int retrievedValue = 0;
                await Task.Run(context.Extend(() => retrievedValue = context.Value));
                Assert.That(retrievedValue, Is.EqualTo(1));
                Assert.That(context.Value, Is.EqualTo(1));
            }
        }

        [Test]
        public void ContextPreservesValueInCoroutine()
        {
            //TODO: do
        }

        [Test]
        public void ContextCanBeBoundToDelegate()
        {
            var context = new Context<int>(5);
            Action callback;
            
            using (context.Provide(6))
            {
                callback = context.Extend(Foo);
            }

            Assert.That(context.Value, Is.EqualTo(5));
            callback();
            return;
            
            void Foo()
            {
                Assert.That(context.Value, Is.EqualTo(6));
            }
        }
    }
}