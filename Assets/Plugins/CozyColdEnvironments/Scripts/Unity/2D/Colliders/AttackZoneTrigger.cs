using CCEnvs.Unity.Extensions;
using CCEnvs.Utils;
using System.Linq;
using UnityEngine;

namespace CCEnvs.Unity.TwoD.Colliders
{
    public class AttackZoneTrigger : CompositeCollider<PolygonCollider2D>,
        ICompositeTrigger<Direction2D, PolygonCollider2D>
    {
        protected override void Awake()
        {
            base.Awake();
            AddColliders();
            ConvertCollidersToTriggers();
            DisableTriggers();
        }

        public void SetColliderShape(IColliderShapeInfo shapeInfo)
        {
            Vector2[] newTriggerShape = PolygonGenerator.GetFourVerticePolygonCoordinates(shapeInfo.NearEdgeDistance, shapeInfo.FarEdgeDistance,
                shapeInfo.Range);

            ApplyColliderShape(newTriggerShape);
        }

        public void Activate(Direction2D value) => Activate((int)value);

        public void ApplyColliderShape(Vector2[] triggerShape)
        {
            for (int i = 0; i < colliders.Length; i++)
                colliders[i].IfNotNull((trigger) => trigger.SetPath(0, triggerShape));
        }

        protected void AddCollider(Direction2D direction)
        {
            colliders[(int)direction] = transform.Find(direction.ToString())
                                                 .IfNotNull((triggerObjTransform) =>
                triggerObjTransform.GetComponent<PolygonCollider2D>());
        }

        protected void AddColliders()
        {
            var directions = EnumHelper.GetValues<Direction2D>();
            colliders = new PolygonCollider2D[directions.Length];

            for (int i = 0; i < directions.Length; i++)
                AddCollider(directions[i]);

            complexity = colliders.Count(CCEnvs.Diagnostics.ObjectExtensions.IsNotNull);
        }

        protected void ConvertCollidersToTriggers()
        {
            for (int i = 0; i < colliders.Length; i++)
                colliders[i].IfNotNull((collider) => collider.isTrigger = true);
        }

        protected void DisableTriggers()
        {
            for (int i = 0; i < complexity; i++)
                colliders[i].IfNotNull((trigger) => trigger.enabled = false);
        }
    }
}