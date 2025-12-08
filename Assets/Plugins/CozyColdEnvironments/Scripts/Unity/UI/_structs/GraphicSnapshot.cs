using System;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity.Snaphots.UI
{
    [Serializable]
    public class GraphicSnapshot : BehaviourSnapshot
    {
        [SerializeField]
        protected Color m_Color;

        [SerializeField]
        protected bool m_RaycastTarget;

        public Color color => m_Color;
        public bool RaycastTarget => m_RaycastTarget;

        public GraphicSnapshot()
        {
        }

        public GraphicSnapshot(Graphic target)
            :
            base(target)
        {
            m_Color =  target.color;
            m_RaycastTarget = target.raycastTarget;
        }

        public override void Restore(object taregt)
        {
            var graphic = ValidateTarget<Graphic>(target);

            graphic.color = color;
            graphic.raycastTarget = RaycastTarget;
        }

        public override string ToString()
        {
            return $"{nameof(Target)}: {Target}; {nameof(Enabled)}: {Enabled}.";
        }
    }
}
