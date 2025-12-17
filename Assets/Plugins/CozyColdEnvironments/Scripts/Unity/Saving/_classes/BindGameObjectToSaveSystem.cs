using CCEnvs.Unity.Components;
using CCEnvs.Unity.Saving;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [RequireComponent(typeof(PersistentGuid))]
    public sealed class BindGameObjectToSaveSystem : CCBehaviour
    {
        [SerializeField]
        [Tooltip("Binds only enumerated components and self game object. Keep empty for auto binding")]
        private Component[]? componentsExplicit;

        [SerializeField]
        private bool ignoreComponents;

        protected override void Awake()
        {
            base.Awake();

            gameObject.BindToSaveSystem().BindDisposableTo(this);

            if (ignoreComponents)
                return;

            if (componentsExplicit is not null)
            {
                foreach (var cmp in componentsExplicit)
                    cmp.BindToSaveSystem().BindDisposableTo(this);
            }
            else
            {
                foreach (var cmp in GetComponents<Component>())
                {
                    if (cmp == this)
                        continue;

                    if (cmp.IsTypeRegisteredInSaveSystem())
                        cmp.BindToSaveSystem().BindDisposableTo(this);
                }
            }
        }
    }
}
