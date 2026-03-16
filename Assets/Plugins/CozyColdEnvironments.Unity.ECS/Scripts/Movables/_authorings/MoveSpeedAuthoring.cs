using Unity.Entities;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.ECS.Movables
{
    public class MoveSpeedAuthoring : MonoBehaviour
    {
        public float MoveSpeed = 1f;

        private class Baker : Baker<MoveSpeedAuthoring>
        {
            public override void Bake(MoveSpeedAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

                var cmp = new MoveSpeed
                {
                    Value = authoring.MoveSpeed
                };

                AddComponent(entity, cmp);
            }
        }
    }
}
