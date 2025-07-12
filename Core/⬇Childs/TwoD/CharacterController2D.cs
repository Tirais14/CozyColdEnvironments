using UnityEngine;
using UTIRLib.Injector;
using UTIRLib.Utils;

#nullable enable

namespace UTIRLib.TwoD
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public sealed class CharacterController2D : MonoX
    {
        [GetComponent]
        private Rigidbody2D rigidbody2DComponent = null!;

        [GetComponent]
        private Collider2D collider2DComponent = null!;

        public Rigidbody2D Rigidbody2D => rigidbody2DComponent;
        public Collider2D Collider2D => collider2DComponent;
        public Vector2 Position => transform.position;

        protected override void OnStart()
        {
            base.OnStart();
            Setup();
        }

        public void Move(Vector2 direction, float speed)
        {
            if (speed <= 0)
            {
                return;
            }

            Vector3 offset = direction * speed;
            Rigidbody2D.MovePosition(transform.position + offset);
        }

        public void MoveTo(Vector2 targetPosition, float speed)
        {
            if (speed <= 0)
            {
                return;
            }

            Vector2 direction = VectorHelper.GetDirection(Position, targetPosition);
            Vector2 offset = direction * speed;
            float sqrDistance = VectorHelper.SqrDistance(Position, targetPosition);
            if (offset.sqrMagnitude >= sqrDistance) { Rigidbody2D.MovePosition(targetPosition); }
            else { Rigidbody2D.MovePosition(Position + offset); }
        }

        private void Setup()
        {
            Rigidbody2D.freezeRotation = true;
            Rigidbody2D.gravityScale = 0;
        }
    }
}