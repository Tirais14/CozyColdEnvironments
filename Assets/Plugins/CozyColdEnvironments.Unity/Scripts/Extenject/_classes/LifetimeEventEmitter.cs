using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

#nullable enable
namespace CCEnvs.Zenject
{
    internal class LifetimeEventEmitter : MonoBehaviour, IInitializable
    {
        private List<IStartable>? startables;

        public DiContainer diContainer;

        void IInitializable.Initialize()
        {
            startables = diContainer.ResolveAll<IStartable>();
            enabled = true;
        }

        private void Start()
        {
            if (startables == null)
                return;

            for (int i = 0; i < startables.Count; i++)
            {
                if (startables[i].IsNull(out var startable))
                    continue;

                try
                {
                    startable.Start();
                }
                catch (Exception ex)
                {
                    this.PrintException(ex);
                }
            }

            Destroy(gameObject);
        }
    }
}
