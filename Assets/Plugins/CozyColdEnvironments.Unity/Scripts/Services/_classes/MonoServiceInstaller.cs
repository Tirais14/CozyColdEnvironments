using CCEnvs.Services;
using CCEnvs.UnityX.Components;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.Services
{
    [DefaultExecutionOrder(-9999)]
    public abstract class MonoServiceInstaller : CCBehaviour, IServiceInstaller
    {
        protected override void Awake()
        {
            base.Awake();
            InstallBindings();
        }

        protected abstract void InstallBindings();
    }
}
