using CCEnvs.Diagnostics;
using CCEnvs.Unity.Initables;
using Cysharp.Threading.Tasks;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    public abstract class AEntryPointBehaviour : CCBehaviourStatic
    {
        private int frames;

        protected override void OnAwake()
        {
            base.OnAwake();
            Install.Static();

            SceneInitializer.InitAllObjectsAsync(FindObjectsInactive.Exclude)
                .Forget(ex => this.PrintException(ex));
        }

        private void Update()
        {
            if (frames >= 10) //some frames delay
                Destroy(this);

            frames++;
        }
    }
}
