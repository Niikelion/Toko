using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Toko.Core
{
    [PublicAPI]
    public class MonoBehaviourWithResources : MonoBehaviour
    {
        private List<IDisposable> resources = new ();
        private HashSet<IDisposable> resourceSet = new ();

        public virtual void OnDestroy() => ReleaseResources();
        public void Use(IDisposable disposable)
        {
            if (!resourceSet.Add(disposable)) return;
            resources.Add(disposable);
        }

        protected void ReleaseResources()
        {
            resources.Reverse();
            foreach (var resource in resources) resource.Dispose();
            resources.Clear();
            resourceSet.Clear();
        }
    }
}