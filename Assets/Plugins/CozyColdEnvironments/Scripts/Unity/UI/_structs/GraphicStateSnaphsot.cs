using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public readonly struct GraphicStateSnaphsot : ISnapshot<Graphic>
    {
        public Graphic Target { get; }
        public Color color { get; }
        public bool RaycastTarget { get; }
        public bool Enabled { get; }

        public GraphicStateSnaphsot(Graphic target)
            :
            this()
        {
            CC.Guard.IsNotNull(target, nameof(target));

            Target = target;
            color =  target.color;
            RaycastTarget = target.raycastTarget;
            Enabled = target.enabled;
        }

        public static implicit operator GraphicStateSnaphsot(Graphic graphic)
        {
            return new GraphicStateSnaphsot(graphic);
        }

        public void Restore()
        {
            if (Target == null)
                return;

            Target.color = color;
            Target.raycastTarget = RaycastTarget;
            Target.enabled = Enabled;
        }

        public override string ToString()
        {
            return $"{nameof(Target)}: {Target}; {nameof(Enabled)}: {Enabled}.";
        }
    }

    public static class GraphicStateSnaphsotExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GraphicStateSnaphsot CaptureState(this Graphic graphic)
        {
            CC.Guard.IsNotNull(graphic, nameof(graphic));
            return graphic;
        }
    }
}
