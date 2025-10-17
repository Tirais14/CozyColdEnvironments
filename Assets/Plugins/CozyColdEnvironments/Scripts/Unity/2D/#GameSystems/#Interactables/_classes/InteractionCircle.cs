using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.GameSystems.Interactables
{
    public class InteractionCircle : InteractionZone
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
