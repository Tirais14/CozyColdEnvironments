using CCEnvs.Snapshots;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity
{
    [Serializable]
    public class ComponentSnapshot : Snapshot<Component>
    {
        public ComponentSnapshot()
        {
        }

        public ComponentSnapshot(Component target)
            :
            base(target)
        {
        }

        public override Component Restore(Component target)
        {
            return target;
        }
    }
}
