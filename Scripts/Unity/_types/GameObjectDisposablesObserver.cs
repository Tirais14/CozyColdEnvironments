using UnityEngine;
using CCEnvs.Diagnostics;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Unity.Disposables
{
    public sealed class GameObjectDisposablesObserver : CCBehaviour
    {
        public const int SKIP_FRAMES_COUNT = 9;

        private readonly List<DisposableBinding> bindings = new();
        private int bindingsCount;
        private int frameCounter = 0;

        private void LateUpdate()
        {
            if (frameCounter <= SKIP_FRAMES_COUNT)
                return;

            DisposableBinding binding;
            for (int i = 0; i < bindingsCount; i++)
            {
                binding = bindings[i];
                if (binding.ReadyToDispose)
                {
                    binding.Dispose();
                    bindings.RemoveAt(i);
                    bindingsCount = bindings.Count;
                }
            }

            if (bindingsCount == 0)
                Destroy(this);

            frameCounter++;
        }

        private void OnDestroy()
        {
            for (int i = 0; i < bindingsCount; i++)
                bindings[i].Dispose();
        }

        /// <summary>
        /// Bind disposable to component or game object for automatically dispose.
        /// </summary>
        /// <param name="component">if null, binds to game object destroy trigger</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void BindDisposable(IDisposable disposable, Component? component = null)
        {
            if (disposable.IsNull())
                throw new ArgumentNullException(nameof(disposable));

            bindings.Add(new DisposableBinding(component, disposable));
            bindingsCount = bindings.Count;
        }

        /// <summary>
        /// Bind disposable to component or game object for automatically dispose.
        /// </summary>
        /// <param name="component">if null, binds to game object destroy trigger</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="CollectionArgumentException"></exception>
        public void BindDisposables(Component? component,
                                    params IDisposable[] disposables)
        {
            if (disposables.IsNullOrEmpty())
                throw new CollectionArgumentException(nameof(disposables), disposables);

            for (int i = 0; i < disposables.Length; i++)
                bindings.Add(new DisposableBinding(component, disposables[i]));

            bindingsCount = bindings.Count;
        }
        /// <summary>
        /// Bind disposable to game object for automatically dispose.
        /// </summary>
        public void BindDisposables(params IDisposable[] disposables)
        {
            BindDisposables(component: null, disposables);
        }
    }
}
