using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity
{
    public static class LayoutHelper
    {
        public static async UniTask ForceRebuildLayoutAsync(RectTransform source)
        {
            CC.Guard.IsNotNullSource(source);

            await UniTask.NextFrame();
            await UniTask.WaitForEndOfFrame();

            LayoutRebuilder.ForceRebuildLayoutImmediate(source);
        }
    }
}
