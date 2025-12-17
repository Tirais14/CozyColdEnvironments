using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    public class MonoBehaviourSnapshot<T> : BehaviourSnapshot<T>
        where T : MonoBehaviour
    {
        public MonoBehaviourSnapshot()
        {
        }

        public MonoBehaviourSnapshot(T target) : base(target)
        {
        }
    }
}
