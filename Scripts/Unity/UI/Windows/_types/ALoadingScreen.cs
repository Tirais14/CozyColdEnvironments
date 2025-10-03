using CCEnvs.Unity.UI.Elements;
using TMPro;
using UnityEngine;

#nullable enable
#pragma warning disable IDE1006
namespace CCEnvs.Unity.UI.Elements
{
    public abstract class ALoadingScreen : Window, ILoadingScreen
    {
        [field: SerializeField]
        public TextMeshProUGUI TextMesh { get; private set; } = null!;
    }
}
