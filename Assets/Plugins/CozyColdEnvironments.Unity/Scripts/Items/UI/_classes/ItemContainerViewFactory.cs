using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CCEnvs.Patterns.Factories;
using CCEnvs.UnityX.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using ZLinq;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.UnityX.Items.UI
{
    public class ItemContainerViewFactory : IItemContainerViewFactory
    {
        public async ValueTask<IReadOnlyList<GameObject>> Create(
            int count,
            GameObject prefab,
            InstantiateParameters parameters,
            CancellationToken cancellationToken
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (count <= 0)
                return Array.Empty<GameObject>();

            var instances = await Object.InstantiateAsync(
                prefab,
                count,
                parameters,
                cancellationToken: cancellationToken
                );

            var containerViewModels = new List<IItemContainerViewModel>(count);

            for (int i = 0; i < instances.Length; i++)
            {
                try
                {
                    OnInstantiated(instances[i]);
                }
                catch (Exception ex)
                {
                    this.PrintException(ex);
                }
            }

            return instances;
        }

        protected virtual void OnInstantiated(GameObject gameObject)
        {

        }
    }
}
