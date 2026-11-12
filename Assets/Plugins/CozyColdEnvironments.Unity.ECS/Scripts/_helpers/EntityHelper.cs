using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

#nullable enable
namespace CCEnvs.UnityX.ECS
{
    public static class EntityHelper
    {
        public static void SetPosition(
            in EntityCommandBuffer ecb, 
            ComponentLookup<LocalTransform> transformLookup,
            in ComponentLookup<Parent> parentLookup,
            in ComponentLookup<LocalToWorld> localToWorldLookup,
            Entity entity,
            float3 position
            )
        {
            if (!ecb.IsCreated)
                throw new System.ArgumentNullException(nameof(ecb));
            if (entity == Entity.Null)
                throw new System.ArgumentNullException(nameof(entity));

            var transform = transformLookup[entity];
            if (transformLookup.HasComponent(entity))
            {
                var parent = parentLookup[entity];
                var parentLocalToWorld = localToWorldLookup[parent.Value];

                parentLocalToWorld.Value.InverseTransformPoint(position);
                transform.Position = position;
            }
            else
            {
                transform.Position = position;
            }

            ecb.SetComponent(entity, transform);
        }

        public static void SetPosition(
            in EntityCommandBuffer ecb,
            in EntityManager entityManager,
            Entity entity,
            float3 position
            )
        {
            if (!ecb.IsCreated)
                throw new System.ArgumentNullException(nameof(ecb));
            if (entity == Entity.Null)
                throw new System.ArgumentNullException(nameof(entity));

            var transform = entityManager.GetComponentData<LocalTransform>(entity);
            if (entityManager.HasComponent<Parent>(entity))
            {
                var parent = entityManager.GetComponentData<Parent>(entity);
                var parentLocalToWorld = entityManager.GetComponentData<LocalToWorld>(parent.Value);
                transform.Position = parentLocalToWorld.Value.InverseTransformDirection(position);
            }
            else
            {
                transform.Position = position;
            }

            ecb.SetComponent(entity, transform);
        }

        public static void SetPosition(
            in EntityManager entityManager,
            Entity entity,
            float3 position
            )
        {
            if (entity == Entity.Null)
                throw new System.ArgumentNullException(nameof(entity));

            var transform = entityManager.GetComponentData<LocalTransform>(entity);
            if (entityManager.HasComponent<Parent>(entity))
            {
                var parent = entityManager.GetComponentData<Parent>(entity);
                var parentLocalToWorld = entityManager.GetComponentData<LocalToWorld>(parent.Value);
                transform.Position = parentLocalToWorld.Value.InverseTransformPoint(position);
            }
            else
                transform.Position = position;

            entityManager.SetComponentData(entity, transform);
        }
        public static void SetPosition(
            ComponentLookup<LocalTransform> transformLookup,
            in ComponentLookup<LocalToWorld> localToWorldLookup,
            in ComponentLookup<Parent> parentLookup,
            Entity entity,
            float3 position
            )
        {
            if (entity == Entity.Null)
                throw new System.ArgumentNullException(nameof(entity));

            var transform = transformLookup[entity];

            if (parentLookup.HasComponent(entity))
            {
                var parent = parentLookup[entity];
                var parentLocalToWorld = localToWorldLookup[parent.Value];

                transform.Position = parentLocalToWorld.Value.InverseTransformPoint(position);
            }
            else
                transform.Position = position;

            transformLookup[entity] = transform;
        }
    }
}
