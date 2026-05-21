using Cysharp.Threading.Tasks;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX
{
    public sealed class RootMarker : MonoBehaviour
    {
        private void Awake()
        {
            Disable().Forget(ex => this.PrintException(ex));
        }

        private async UniTask Disable()
        {
            await UniTask.DelayFrame(delayFrameCount: 2);
            enabled = false;
        }
    }
}
