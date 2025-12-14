using CCEnvs.Snapshots;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snaphots
{
    [Serializable]
    public class BehaviourSnapshot : Snapshot<Behaviour>
    {
        [SerializeField]
        [JsonProperty("componentSnapshot")]
        protected ComponentSnapshot cmpSnapshot = new(); 

        [SerializeField]
        [JsonProperty("enabled")]
        protected bool enabled;

        public BehaviourSnapshot()
        {
        }

        public BehaviourSnapshot(Behaviour target)
            :
            base(target)
        {
            cmpSnapshot = new ComponentSnapshot(target);
            enabled = target.enabled;
        }

        public override Behaviour Restore(Behaviour target)
        {
            CC.Guard.IsNotNull(target, nameof(target));
            Guard.IsNotNull(cmpSnapshot);

            target = cmpSnapshot.Restore(target).To<Behaviour>();
            target.enabled = enabled;

            return target;
        }
    }
}
