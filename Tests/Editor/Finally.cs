using NUnit.Framework;
using Toko.Core;

namespace Toko.Tests.Editor
{
    [TestFixture]
    public class FinallyFixture
    {
        [Test]
        public void FinallyCallsCallbackOnEveryDispose()
        {
            int counter = 0;
            var f = new Finally(() => counter++);
            Assert.That(counter, Is.EqualTo(0));
            f.Dispose();
            Assert.That(counter, Is.EqualTo(1));
            f.Dispose();
            Assert.That(counter, Is.EqualTo(2));
            f.Dispose();
            Assert.That(counter, Is.EqualTo(3));
        }
    }
}