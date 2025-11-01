using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Interactables
{
    [RequireComponent(typeof(Collider2D))]
    public class InteractionArea : InteractionZone<Collider2D>
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            TryAddInteractableBy(collision);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            TryRemoveInteractableBy(collision);
        }

        public override bool Contains(Vector2 point)
        {
            return agent.OverlapPoint(point);
        }
        public override bool Contains(Vector3 point)
        {
            return Contains((Vector2)point);
        }
    }
}
