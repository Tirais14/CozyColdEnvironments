using System.Linq;
using UnityEngine;
using UTIRLib.Diagnostics;
using UTIRLib.Utils;

namespace UTIRLib.TwoD
{
    public class AttackZoneTrigger : CompositeCollider<PolygonCollider2D>,
        ICompositeTrigger<Direction2D, PolygonCollider2D>
    {
        protected override void OnAwake()
        {
            base.OnAwake();
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

        protected void AddCollider(Direction2D direction) => colliders[(int)direction] = transform.Find(direction).
            IfNotNullQ((triggerObjTransform) => triggerObjTransform.GetComponent<PolygonCollider2D>());

        protected void AddColliders()
        {
            var directions = EnumHelper.GetValues<Direction2D>();
            colliders = new PolygonCollider2D[directions.Length];

            for (int i = 0; i < directions.Length; i++)
                AddCollider(directions[i]);

            complexity = colliders.Count(Diagnostics.ObjectExtensions.IsNotNull);
        }

        protected void ConvertCollidersToTriggers()
        {
            for (int i = 0; i < colliders.Length; i++)
                colliders[i].IfNotNullQ((collider) => collider.isTrigger = true);
        }

        protected void DisableTriggers()
        {
            for (int i = 0; i < complexity; i++)
                colliders[i].IfNotNullQ((trigger) => trigger.enabled = false);
        }
    }
}