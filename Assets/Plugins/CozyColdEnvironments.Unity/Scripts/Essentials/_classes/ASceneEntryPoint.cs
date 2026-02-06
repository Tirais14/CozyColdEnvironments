using CCEnvs.Unity.Components;
using CCEnvs.Unity.Initables;
using Cysharp.Threading.Tasks;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Essentials
{
    public abstract class ASceneEntryPoint : CCBehaviourStatic
    {
        protected override void Awake()
        {
            base.Awake();
            CCProjectHelper.StaticObsolete();
            CC.Install();
            SceneInitializer.InitAllObjectsAsync(FindObjectsInactive.Exclude)
                .Forget(ex => this.PrintException(ex));
        }
    }
}
