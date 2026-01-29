using CCEnvs.Unity.Components;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    public class CCBehaviourSnapshot<T> : MonoBehaviourSnapshot<T>
        where T : CCBehaviour
    {
        public CCBehaviourSnapshot()
        {
        }

        public CCBehaviourSnapshot(T target) : base(target)
        {
        }
    }
}
