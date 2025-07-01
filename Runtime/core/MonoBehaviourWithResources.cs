#if UNITY_6000_0_OR_NEWER

#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toko.Core
{
    public class MonoBehaviourWithResources : MonoBehaviour
    {
        private readonly List<IDisposable> resources = new();
        private readonly HashSet<IDisposable> resourceSet = new();

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
#endif