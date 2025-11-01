using CCEnvs.Unity.Interactables;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity._3D.Interactables
{
    [RequireComponent(typeof(Collider))]
    public class InteractionVolumeByCollider : InteractionZone<Collider>
    {
        private void OnTriggerEnter(Collider other)
        {
            otherAgents.Add(other);
        }

        private void OnTriggerExit(Collider other)
        {
            otherAgents.Remove(other);
        }

        public override bool Contains(Vector2 point)
        {
            return Contains((Vector3)point);
        }

        public override bool Contains(Vector3 point)
        {
            return agent.bounds.Contains(point);
        }
    }
}
