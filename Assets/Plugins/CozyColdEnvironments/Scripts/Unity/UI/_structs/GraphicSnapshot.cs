using CCEnvs.Snapshots;
using CommunityToolkit.Diagnostics;
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
        protected BehaviourSnapshot? behSnapshot;

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
            behSnapshot = new BehaviourSnapshot(target);
            color =  target.color;
            raycastTarget = target.raycastTarget;
        }

        public override Graphic Restore(Graphic target)
        {
            CC.Guard.IsNotNullTarget(target);
            Guard.IsNotNull(behSnapshot);

            behSnapshot.Restore(target);
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
