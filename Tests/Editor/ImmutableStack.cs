using System.Collections.Generic;
using NUnit.Framework;
using Toko.Core;

namespace Toko.Tests.Editor
{
    [TestFixture]
    public class ImmutableStackFixture
    {
        private static IEnumerable<T> Seq<T>(params T[] items) => items;
        
        [Test]
        public void EmptyStackIsSingletonAndEmpty()
        {
            var e1 = ImmutableStack<int>.Empty;
            Assert.That(e1.IsEmpty, Is.True);
            Assert.That(e1, Is.EqualTo(ImmutableStack<int>.Empty));
        }

        [Test]
        public void PushesReturnNewCollectionWithoutModifyingOriginal()
        {
            var e1 = ImmutableStack<int>.Empty.Push(5);
            var e2 = e1.Push(1).Push(3);
            
            Assert.That(e1, Is.EqualTo(Seq(5)).AsCollection);
            Assert.That(e2, Is.EqualTo(Seq(3, 1, 5)).AsCollection);
        }

        [Test]
        public void PopsReturnNewCollectionWithModifyingOriginal()
        {
            var e1 = ImmutableStack<int>.Empty.Push(5).Push(3).Push(1);
            var e2 = e1.Pop().Pop();
            
            Assert.That(e1, Is.EqualTo(Seq(1, 3, 5)).AsCollection);
            Assert.That(e2, Is.EqualTo(Seq(5)).AsCollection);
        }

        [Test]
        public void CanPeekValues()
        {
            var e1 = ImmutableStack<int>.Empty.Push(5);
            Assert.That(e1.Peek(), Is.EqualTo(5));
            e1 = e1.Push(4);
            Assert.That(e1.Peek(), Is.EqualTo(4));
            e1 = e1.Pop();
            Assert.That(e1.Peek(), Is.EqualTo(5));
            e1 = e1.Pop();
            Assert.That(() => e1.Peek(), Throws.InvalidOperationException);
        }
    }
}