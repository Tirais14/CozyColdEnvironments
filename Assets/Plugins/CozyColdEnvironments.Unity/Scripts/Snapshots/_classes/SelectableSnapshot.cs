using System;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public class SelectableSnapshot<T> : UIBehaviourSnapshot<T>
        where T : Selectable
    {
        public SelectableSnapshot()
        {
        }

        public SelectableSnapshot(T target) : base(target)
        {
        }
    }
}
