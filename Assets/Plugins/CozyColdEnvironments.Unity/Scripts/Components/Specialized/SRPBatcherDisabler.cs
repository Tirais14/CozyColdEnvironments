using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using Cysharp.Threading.Tasks;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshRenderer))]
    public class SRPBatcherDisabler : CCBehaviour
    {
        [GetBySelf]
        private MeshRenderer meshRenderer = null!;

        protected override void Start()
        {
            base.Start();
            SetPropertyBlock().Forget();
        }

        private async UniTaskVoid SetPropertyBlock()
        {
            try
            {
                await UniTask.DelayFrame(6, cancellationToken: destroyCancellationToken);

                var propBlock = new MaterialPropertyBlock();
                meshRenderer.SetPropertyBlock(propBlock);

                Destroy(this);
            }
            catch (System.Exception ex)
            {
                if (ex.IsOperationCanceledException())
                    this.PrintExceptionAsLog(ex);

                this.PrintException(ex);
            }
        }
    }
}