using CCEnvs.Snapshots;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public class ComponentSnapshot<T> : Snapshot<T>
        where T : Component
    {
        public GameObjectExtraInfo ExtraInfo { get; set; }

        public ComponentSnapshot()
        {
        }

        public ComponentSnapshot(T target)
            :
            base(target)
        {
            ExtraInfo = target.GetExtraInfo();
        }

        public override T Restore(T target)
        {
            CC.Guard.IsNotNullTarget(target);
            return target;
        }
    }
}
