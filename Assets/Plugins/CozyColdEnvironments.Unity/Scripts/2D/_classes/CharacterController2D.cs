using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using UnityEngine;

#nullable enable
#pragma warning disable IDE1006
namespace CCEnvs.Unity._2D
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public abstract class CharacterController2D : CCBehaviour
    {
        [Min(0f)]
        [SerializeField]
        protected float moveSpeed;

        [field: GetBySelf]
        protected Rigidbody2D rb { get; private set; } = null!;

        [field: GetBySelf]
        protected Collider2D col { get; private set; } = null!;

        public virtual float MoveSpeed {
            get => moveSpeed;
            set
            {
                if (value < 0f)
                {
                    this.PrintWarning($"{nameof(MoveSpeed)} cannot be {MoveSpeed}");
                    value = 0f;
                }

                moveSpeed = value;
            }
        }

        protected abstract Vector2 InputValue { get; }

        protected virtual void FixedUpdate()
        {
            ObjectMove.ByPhysics(rb,
                                 InputValue.normalized,
                                 MoveSpeed,
                                 Time.fixedDeltaTime);
        }
    }
}