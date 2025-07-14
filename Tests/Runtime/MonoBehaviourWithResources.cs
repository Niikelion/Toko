#if UNITY_2022_3_OR_NEWER

using NUnit.Framework;
using Toko.Core;
using Toko.Core.Signals;
using UnityEngine;
using Object = UnityEngine.Object;

public class ExampleBehaviour : MonoBehaviourWithResources
{
    public Var<bool> SomeValue { get; } = new(false);

    private void Awake() => Use(SomeValue);
}

namespace Toko.Tests.Runtime
{
    [TestFixture]
    public class MonoBehaviourWithResourcesFixture
    {
        [Test]
        public void ResourcesAreDisposedOnDestroy()
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<ExampleBehaviour>();
            int counter = 0;
            var v = obj.SomeValue;
            v.Value = false;
            v.Event += Increment;
            
            Assert.That(counter, Is.EqualTo(0));
            v.Value = true;
            Assert.That(counter, Is.EqualTo(1));
            Object.DestroyImmediate(obj.gameObject);
            v.Value = false;
            Assert.That(counter, Is.EqualTo(1));
            
            return;
            
            void Increment(bool _) => counter++;
        }
    }
}

#endif