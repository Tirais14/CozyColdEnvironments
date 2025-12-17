using CCEnvs.Snapshots;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    [Serializable]
    public class ComponentSnapshot<T> : Snapshot<T>
        where T : Component
    {
        public ComponentSnapshot()
        {
        }

        public ComponentSnapshot(T target)
            :
            base(target)
        {
        }

        public override T Restore(T? target)
        {
            CC.Guard.IsNotNullTarget(target);

            return target;
        }
    }

    [Serializable]
    public class ComponentSnapshot : ComponentSnapshot<Component>
    {
    }
}
