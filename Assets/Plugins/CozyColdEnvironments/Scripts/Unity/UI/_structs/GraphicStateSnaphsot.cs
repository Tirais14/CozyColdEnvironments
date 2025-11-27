using System;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public readonly struct GraphicStateSnaphsot : ISnapshot<Graphic>
    {
        public Graphic Target { get; }
        public float ColorAlpha { get; }
        public bool RaycastTarget { get; }
        public bool Enabled { get; }

        public GraphicStateSnaphsot(Graphic target)
            :
            this()
        {
            CC.Guard.IsNotNull(target, nameof(target));

            Target = target;
            ColorAlpha =  target.color.a;
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

            Target.color = Target.color.WithAlpha(ColorAlpha);
            Target.raycastTarget = RaycastTarget;
            Target.enabled = Enabled;
        }

        public override int GetHashCode() => HashCode.Combine(Target);

        public override string ToString()
        {
            return $"{nameof(Target)}: {Target}; {nameof(Enabled)}: {Enabled}.";
        }
    }
}
