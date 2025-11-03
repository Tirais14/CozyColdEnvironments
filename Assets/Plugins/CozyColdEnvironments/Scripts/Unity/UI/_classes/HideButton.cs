using System;
using UnityEngine;
using UnityEngine.Events;
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

            target = this.FindFor()
                         .InParent()
                         .IncludeInactive()
                         .Component<IViewElement>()
                         .Strict();
        }

        protected override void Start()
        {
            base.Start();

            if (!Application.isPlaying)
                return;

            onClick.AddListener(target.Hide);
        }

        protected override void OnDestroy()
        {
            onClick.RemoveListener(target.Hide);
        }
    }
}
