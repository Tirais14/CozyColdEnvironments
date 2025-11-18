using UnityEngine;
using UnityEngine.UI;

#nullable enable
#pragma warning disable IDE0044
#pragma warning disable IDE1006
#pragma warning disable IDE0051
#pragma warning disable S3236
namespace CCEnvs.Unity.UI.Elements
{
    [RequireComponent(typeof(Image))]
    public sealed class HideButton : Button
    {
        private IViewElement target = null!;

        protected override void Awake()
        {
            base.Awake();

            if (!Application.isPlaying)
                return;

            target = this.QueryTo()
                         .ByParent()
                         .IncludeInactive()
                         .Component<IViewElement>()
                         .Strict();
        }

        protected override void Start()
        {
            base.Start();
            onClick.AddListener(target.Hide);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            onClick.RemoveListener(target.Hide);
        }
    }
}
