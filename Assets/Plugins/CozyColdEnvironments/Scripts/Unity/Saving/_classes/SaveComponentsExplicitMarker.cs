using CCEnvs.Unity.Components;
using Cysharp.Threading.Tasks;
using System;
using R3;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Saving
{
    public sealed class SaveComponentsExplicitMarker : CCBehaviour
    {
        public Component[] components = Array.Empty<Component>();

        protected override void Start()
        {
            base.Start();

            foreach (var cmp in components)
                cmp.BindToSaveSystem().AddTo(cmp.GetCancellationTokenOnDestroy());

            Destroy(this);
        }
    }
}
