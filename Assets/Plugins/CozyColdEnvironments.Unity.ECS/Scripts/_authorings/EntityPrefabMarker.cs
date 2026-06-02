using Unity.Entities;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.ECS
{
    public sealed class EntityPrefabMarker : MonoBehaviour
    {
        public class Baker : Baker<EntityPrefabMarker>
        {
            public override void Bake(EntityPrefabMarker authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.None);
                AddComponent<Prefab>(entity);
            }
        }
    }
}
