using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CCEnvs.Unity.UI
{
    public readonly struct BeforeDisabledGraphicSnapshot
    {
        public Graphic Target { get; }
        public Color Color { get; }
        public bool RaycastTarget { get; }

        public BeforeDisabledGraphicSnapshot(Graphic target)
        {
            CC.Guard.IsNotNull(target, nameof(target));

            Target = target;
            RaycastTarget = target.raycastTarget;
            Color = target.color;
        }

        public void Apply()
        {
            Target.raycastTarget = RaycastTarget;
            Target.color = Color;
        }
    }
}
