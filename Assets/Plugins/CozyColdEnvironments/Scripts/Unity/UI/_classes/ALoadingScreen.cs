using TMPro;
using UnityEngine;

#nullable enable
#pragma warning disable IDE1006
namespace CCEnvs.Unity.UI
{
    public abstract class ALoadingScreen : GUITab, ILoadingScreen
    {
        [field: SerializeField]
        public TextMeshProUGUI TextMesh { get; private set; } = null!;
    }
}
