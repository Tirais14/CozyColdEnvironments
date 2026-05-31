using Unity.Entities;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.ECS
{
    public sealed class PrefabAuthoring : MonoBehaviour
    {
        public class Baker : Baker<PrefabAuthoring>
        {
            public override void Bake(PrefabAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.None);
                AddComponent<Prefab>(entity);
            }
        }
    }
}
