using CCEnvs.Unity.UI.Windows;
using TMPro;
using UnityEngine;

#nullable enable
#pragma warning disable IDE1006
namespace CCEnvs.Unity.UI.Windows
{
    public abstract class ALoadingScreen : Window, ILoadingScreen
    {
        [field: SerializeField]
        public TextMeshProUGUI TextMesh { get; private set; } = null!;
    }
}
