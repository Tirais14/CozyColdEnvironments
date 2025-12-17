using CCEnvs.Snapshots;
using CommunityToolkit.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public class BehaviourSnapshot<T> : ComponentSnapshot<T>
        where T : Behaviour
    {
        [JsonInclude]
        [SerializeField]
        protected bool enabled;

        public BehaviourSnapshot()
        {
        }

        public BehaviourSnapshot(T target)
            :
            base(target)
        {
            enabled = target.enabled;
        }

        [JsonConstructor]
        public BehaviourSnapshot(bool enabled)
        {
            this.enabled = enabled;
        }

        public override T Restore(T? target)
        {
            base.Restore(target);
            target = base.Restore(target);

            CC.Guard.IsNotNull(target, nameof(target));

            target.enabled = enabled;

            return target;
        }
    }
}
