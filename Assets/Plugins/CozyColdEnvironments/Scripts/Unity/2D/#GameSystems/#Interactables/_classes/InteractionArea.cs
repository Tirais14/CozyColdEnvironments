using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Interactables
{
    [RequireComponent(typeof(Collider2D))]
    public class InteractionArea : InteractionZone<Collider2D>
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            otherAgents.Add(collision);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            otherAgents.Remove(collision);
        }

        public override bool Contains(Vector2 point)
        {
            return agent.OverlapPoint(point);
        }
        public override bool Contains(Vector3 point)
        {
            return Contains(new Vector2(point.x, point.y));
        }
    }
}
