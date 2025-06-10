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
        public void Use(params IDisposable[] disposables)
        {
            foreach (var disposable in disposables)
            {
                if (!resourceSet.Add(disposable)) continue;
                resources.Add(disposable);
            }
        }

        public void Dispose(params IDisposable[] disposables)
        {
            foreach (var disposable in disposables)
            {
                if (!resourceSet.Remove(disposable)) continue;
                disposable.Dispose();
            }
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