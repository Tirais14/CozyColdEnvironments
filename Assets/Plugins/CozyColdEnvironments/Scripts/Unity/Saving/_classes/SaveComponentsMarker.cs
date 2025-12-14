using CCEnvs.Unity.Components;
using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Saving
{
    public sealed class SaveComponentsMarker : CCBehaviour
    {
        public Component[] components = Array.Empty<Component>();

        protected override void Start()
        {
            base.Start();

            foreach (var cmp in components)
            {
                if (cmp.IsTypeRegisteredInSaveSystem())
                    cmp.BindToSaveSystem().AddTo(gameObject);
            }

            Destroy(this);
        }
    }
}
