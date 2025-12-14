using CCEnvs.Snapshots;
using Newtonsoft.Json;
using System;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity.Snaphots.UI
{
    [Serializable]
    public class GraphicSnapshot : Snapshot<Graphic>
    {
        [SerializeField]
        [JsonProperty("behaviourSnapshot")]
        protected BehaviourSnapshot behaviourSnapshot = new();

        [SerializeField]
        [JsonProperty("behaviourSnapshot")]
        protected Color color;

        [SerializeField]
        [JsonProperty("behaviourSnapshot")]
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

        public override Graphic Restore(Graphic target)
        {
            CC.Guard.IsNotNullTarget(target);

            target = behaviourSnapshot.Restore(target).To<Graphic>();
            target.color = color;
            target.raycastTarget = raycastTarget;

            return target;
        }
    }
}
