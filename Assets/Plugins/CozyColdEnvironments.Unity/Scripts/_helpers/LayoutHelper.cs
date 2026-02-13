using CCEnvs.Threading;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity
{
    public static class LayoutHelper
    {
        public static async UniTask ForceRebuildLayoutAsync(
            RectTransform rectTransform, 
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

            CC.Guard.IsNotNull(rectTransform, nameof(rectTransform));

            await UniTask.NextFrame(cancellationToken: cancellationToken);
            await UniTask.WaitForEndOfFrame(cancellationToken: cancellationToken);

            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }

        public static async UniTask ForceRebuildLayoutsAsync(
            IEnumerable<RectTransform> rectTransforms,
            CancellationToken cancellationToken = default
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

            CC.Guard.IsNotNull(rectTransforms, nameof(rectTransforms));

            await UniTask.NextFrame(cancellationToken: cancellationToken);
            await UniTask.WaitForEndOfFrame(cancellationToken: cancellationToken);

            int i = 0;

            foreach (var rectTransform in rectTransforms)
            {
                cancellationToken.ThrowIfCancellationRequestedByIntervalAndMoveNext(ref i);

                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            }
        }
    }
}
