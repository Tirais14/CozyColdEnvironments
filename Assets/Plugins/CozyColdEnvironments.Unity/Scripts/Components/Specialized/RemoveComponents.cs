#nullable enable
using CCEnvs.Collections;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace CCEnvs.Unity.Components.Specialized
{
    public sealed class RemoveComponents : MonoBehaviour
    {
        public Component[] components = Array.Empty<Component>();
        public PlayerLoopTiming timing = PlayerLoopTiming.Update;
        public int delayFrameCount = 1;

        [Tooltip("In Seconds")]
        public float delayTime = 0f;

        private void Awake()
        {
            if (components.IsNullOrEmpty())
                return;

            UniTask.Create(this,
                static async @this =>
                {
                    await UniTask.Yield(@this.timing);

                    if (@this.delayFrameCount > 0)
                        await UniTask.DelayFrame(@this.delayFrameCount);

                    foreach (var cmp in @this.components)
                        Destroy(cmp, @this.delayTime);

                    Destroy(@this);
                })
                .Forget();
        }
    }
}
