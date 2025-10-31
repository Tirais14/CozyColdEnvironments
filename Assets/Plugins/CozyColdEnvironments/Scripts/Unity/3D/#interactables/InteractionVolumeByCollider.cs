using CCEnvs.Unity.GameSystems.Interactables;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity._3D.Interactables
{
    [RequireComponent(typeof(Collider))]
    public class InteractionVolumeByCollider : InteractionZone<Collider>
    {
        public override bool Contains(Vector2 point)
        {
            return Contains((Vector3)point);
        }

        public override bool Contains(Vector3 point)
        {
            return agent.bounds.Contains(point);
        }

        private void OnTriggerEnter(Collider other)
        {
            TryAddInteractableBy(other);
        }

        private void OnTriggerExit(Collider other)
        {
            TryRemoveInteractableBy(other);
        }
    }
}
