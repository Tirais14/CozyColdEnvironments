using System;
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

        public override Graphic Restore(Graphic target)
        {
            base.Restore(target);
            CC.Guard.IsNotNullTarget(target);

            target.color = Color;
            target.raycastTarget = RaycastTarget;

            return target;
        }
    }
}
