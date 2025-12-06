using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public class GraphicSnapshot : BehaviourSnapshot
    {
        public Color color { get; }
        public bool RaycastTarget { get; }

        new protected Graphic target => (Graphic)base.target;

        public GraphicSnapshot(Graphic target)
            :
            base(target)
        {
            CC.Guard.IsNotNull(target, nameof(target));

            color =  target.color;
            RaycastTarget = target.raycastTarget;
        }

        public static implicit operator GraphicSnapshot(Graphic graphic)
        {
            return new GraphicSnapshot(graphic);
        }

        public override void Restore()
        {
            base.Restore();
            if (Target == null)
                return;

            target.color = color;
            target.raycastTarget = RaycastTarget;
        }

        public override string ToString()
        {
            return $"{nameof(Target)}: {Target}; {nameof(Enabled)}: {Enabled}.";
        }
    }

    public static class GraphicStateSnaphsotExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GraphicSnapshot CaptureState(this Graphic graphic)
        {
            CC.Guard.IsNotNull(graphic, nameof(graphic));
            return graphic;
        }
    }
}
