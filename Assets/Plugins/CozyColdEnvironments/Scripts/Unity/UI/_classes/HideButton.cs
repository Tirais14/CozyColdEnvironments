using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
#pragma warning disable IDE0044
namespace CCEnvs.Unity.UI
{
    [RequireComponent(typeof(Button))]
    public sealed class HideButton : CCBehaviour
    {
        [GetBySelf]
        private Button button = null!;

        [GetByParent]
        private IGUITab target = null!;

        protected override void Start()
        {
            base.Start();

            if (!Application.isPlaying)
                return;

            button.onClick.AddListener(target.Hide);
        }

        private void OnDestroy()
        {
            if (!Application.isPlaying)
                return;

            button.onClick.RemoveListener(target.Hide);
        }
    }
}
