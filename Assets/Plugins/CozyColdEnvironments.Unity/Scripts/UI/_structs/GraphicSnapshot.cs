using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity.Snapshots.UI
{
    [Serializable]
    public class GraphicSnapshot : BehaviourSnapshot<Graphic>
    {
        public Color Color { get; set; }
        public bool RaycastTarget { get; set; }

        public GraphicSnapshot()
        {
        }

        public GraphicSnapshot(Graphic target)
            :
            base(target)
        {
            Color =  target.color;
            RaycastTarget = target.raycastTarget;
        }

        public override bool TryRestore(Graphic? target, [NotNullWhen(true)] out Graphic? restored)
        {
            if (!base.TryRestore(target, out restored))
                return false;

            target!.color = Color;
            target.raycastTarget = RaycastTarget;

            restored = target;
            return true;
        }
    }
}
