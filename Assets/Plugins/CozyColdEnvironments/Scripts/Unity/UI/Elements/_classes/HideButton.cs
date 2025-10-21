using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
#pragma warning disable IDE0044
#pragma warning disable IDE1006
#pragma warning disable IDE0051
#pragma warning disable S3236
namespace CCEnvs.Unity.UI.Elements
{
    [RequireComponent(typeof(Button))]
    public sealed class HideButton : CCBehaviour
    {
        [GetBySelf]
        private Button button = null!;

        [GetByParent]
        private IShowable showable = null!;

        protected override void Start()
        {
            base.Start();

            button.onClick.AddListener(showable.Hide);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
        }
    }
}
