using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.ECS.Characters
{
    public class CharacterAuthoring : MonoBehaviour
    {
        [Header("Parameters")]
        public float MoveSpeed = 1f;

        [Header("Physics")]
        public float Gravity = -9.81f;
        public float GroundCastDistance = 0.007f;

        public CapsuleCollider Collider = null!;

        private void OnDrawGizmos()
        {
            if (Collider == null)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position.AddY(-(Collider.height / 2) + Collider.radius), Collider.radius * 0.9f);
        }

        public class Baker : Baker<CharacterAuthoring>
        {
            public override void Bake(CharacterAuthoring authoring)
            {
                CC.Guard.IsNotNull(authoring.Collider, nameof(Collider));

                Entity e = GetEntity(authoring, TransformUsageFlags.Dynamic);

                AddComponent(
                    e,
                    new CharacterMoveSpeed
                    {
                        Value = authoring.MoveSpeed
                    });

                AddComponent<CharacterInputs>(e);
                AddComponent<CharacterMoveDirection>(e);
                AddComponent(e, CharacterState.Default);
                AddComponent<CharacterVelocity>(e);

                AddComponent(
                    e,
                    new CharacterGravity
                    {
                        Value = authoring.Gravity
                    });

                AddSharedComponent(e, CharacterStates.Default);

                AddSharedComponent(
                    e,
                    new CharacterColliderInfo
                    {
                        Height = authoring.Collider.height,
                        Radius = authoring.Collider.radius
                    });

                float bottomColliderPoint = -(authoring.Collider.height * authoring.transform.localScale.y / 2) + authoring.Collider.radius;

                AddSharedComponent(
                    e,
                    new CharacterGroundCastInfo
                    {
                        CastPoint = new float3(0f, bottomColliderPoint, 0f),
                        CastDistance = authoring.GroundCastDistance,
                        Filter = new Unity.Physics.CollisionFilter
                        {
                            BelongsTo = 1u << authoring.gameObject.layer,
                            CollidesWith = ~0u ^ (1u << authoring.gameObject.layer)
                        }
                    });

                AddComponent<CharacterGroundCastResult>(e);
                AddComponent(e, CharacterRotation.Default); 
            }
        }
    }
}
