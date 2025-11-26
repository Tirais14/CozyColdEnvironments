using CCEnvs.FuncLanguage;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CCEnvs.Unity.UI
{
    public readonly struct GraphicStateSnaphsot
    {
        public Graphic graphic { get; }
        public Color color { get; }
        public bool RaycastTarget { get; }
        public bool Enabled { get; }
        public Maybe<bool> IsVisible { get; }

        public GraphicStateSnaphsot(Graphic target)
            :
            this()
        {
            CC.Guard.IsNotNull(target, nameof(target));

            graphic = target;
            color =  target.color;
            RaycastTarget = target.raycastTarget;
            Enabled = target.enabled;

            if (target.Q().Component<IShowable>().Lax().TryGetValue(out var showable))
                IsVisible = showable.IsVisible;
        }

        public static implicit operator GraphicStateSnaphsot(Graphic graphic)
        {
            return new GraphicStateSnaphsot(graphic);
        }

        public void Restore()
        {
            if (graphic == null)
                return;

            graphic.enabled = Enabled;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(graphic);
        }

        public override string ToString()
        {
            return $"{nameof(graphic)}: {graphic}; {nameof(Enabled)}: {Enabled}.";
        }
    }
}
