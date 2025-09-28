using CCEnvs.Diagnostics;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Initables;
using Cysharp.Threading.Tasks;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Essentials
{
    public abstract class ASceneEntryPoint : CCBehaviourStatic
    {
        protected override void OnAwake()
        {
            base.OnAwake();
            Install.Static();

            SceneInitializer.InitAllObjectsAsync(FindObjectsInactive.Exclude)
                .Forget(ex => this.PrintException(ex));
        }
    }
}
