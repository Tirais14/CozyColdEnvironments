using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.GameSystems.Interactables
{
    [RequireComponent(typeof(Collider2D))]
    public class InteractionAreaCollider : InteractionZone<Collider2D>
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            TryAddInteractableBy(collision);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            TryRemoveInteractableBy(collision);
        }
    }
}
