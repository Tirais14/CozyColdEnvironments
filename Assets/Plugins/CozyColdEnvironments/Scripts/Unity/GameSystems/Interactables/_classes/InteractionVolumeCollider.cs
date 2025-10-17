using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.GameSystems.Interactables
{
    [RequireComponent(typeof(Collider))]
    public class InteractionVolumeCollider : InteractionZone<Collider>
    {
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
