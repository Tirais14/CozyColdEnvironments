using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Interactables
{
    public class InteractionZone2D : InteractionZone<Collider2D>
    {
        protected override void Start()
        {
            base.Start();
            InteractionAgent.isTrigger = true;
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            OnEnter(other.gameObject);
        }

        protected virtual void OnTriggerStay2D(Collider2D other)
        {
            OnStay(other.gameObject);
        }

        protected virtual void OnTriggerExit2D(Collider2D other)
        {
            OnExit(other.gameObject);
        }

        public override bool Contains(Vector2 point)
        {
            return InteractionAgent.OverlapPoint(point);
        }

        public override bool Contains(Vector3 point)
        {
            return Contains((Vector2)point);
        }
    }
}
