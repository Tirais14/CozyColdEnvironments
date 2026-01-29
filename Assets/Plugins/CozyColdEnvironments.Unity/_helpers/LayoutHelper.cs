using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity
{
    public static class LayoutHelper
    {
        public static async UniTask ForceRebuildLayoutAsync(RectTransform rectTransform)
        {
            CC.Guard.IsNotNull(rectTransform, nameof(rectTransform));

            await UniTask.NextFrame();
            await UniTask.WaitForEndOfFrame();

            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }

        public static async UniTask ForceRebuildLayoutsAsync(IEnumerable<RectTransform> rectTransforms)
        {
            CC.Guard.IsNotNull(rectTransforms, nameof(rectTransforms));

            await UniTask.NextFrame();
            await UniTask.WaitForEndOfFrame();

            foreach (var rectTransform in rectTransforms)
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
    }
}
