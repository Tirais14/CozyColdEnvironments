using UnityEngine;
using UnityEngine.UI;

namespace CCEnvs.Unity.UI
{
    public readonly struct GraphicComponentStateSnapshot
    {
        public Graphic Target { get; }
        public Color Color { get; }
        public bool RaycastTarget { get; }
        public bool enabled { get; }

        public GraphicComponentStateSnapshot(Graphic target)
        {
            CC.Guard.IsNotNull(target, nameof(target));

            Target = target;
            RaycastTarget = target.raycastTarget;
            Color = target.color;
            enabled = target.enabled;
        }

        public void Restore()
        {
            if (Target == null)
                return;

            Target.raycastTarget = RaycastTarget;
            Target.color = Color;
            Target.enabled = enabled;
        }
    }
}
