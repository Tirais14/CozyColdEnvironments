using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    [RequireComponent(typeof(MeshRenderer))]
    public class SRPBatcherDisabler : CCBehaviour
    {
        [GetBySelf]
        private MeshRenderer meshRenderer = null!;

        protected override void Start()
        {
            base.Start();

            var propBlock = new MaterialPropertyBlock();
            meshRenderer.SetPropertyBlock(propBlock);

            Destroy(this);
        }
    }
}
