using UnityEngine.EventSystems;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    public class UIBehaviourSnapshot<TThis> : MonoBehaviourSnapshot<TThis>
        where TThis : UIBehaviour
    {
        public UIBehaviourSnapshot()
        {
        }

        public UIBehaviourSnapshot(TThis target) : base(target)
        {
        }
    }
}
