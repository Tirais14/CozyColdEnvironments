using System;
using System.Text.Json.Serialization;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity.Snaphots.UI
{
    [Serializable]
    public class GraphicSnapshot : BehaviourSnapshot<Graphic>
    {
        [JsonInclude]
        [SerializeField]
        protected Color color;

        [JsonInclude]
        [SerializeField]
        protected bool raycastTarget;

        public GraphicSnapshot()
        {
        }

        public GraphicSnapshot(Graphic target)
            :
            base(target)
        {
            color =  target.color;
            raycastTarget = target.raycastTarget;
        }

        [JsonConstructor]
        public GraphicSnapshot(Color color, bool raycastTarget)
        {
            this.color = color;
            this.raycastTarget = raycastTarget;
        }

        public override Graphic Restore(Graphic? target)
        {
            base.Restore(target);

            CC.Guard.IsNotNullTarget(target);

            target.color = color;
            target.raycastTarget = raycastTarget;

            return target;
        }
    }

    public static class GraphicSnapshotExtensions
    {
        public static GraphicSnapshot CaptureState(this Graphic source)
        {
            CC.Guard.IsNotNullSource(source);
            return new GraphicSnapshot(source);
        }
    }
}
