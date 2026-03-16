using System;
using Unity.Entities;
using Unity.Mathematics;

#nullable enable
namespace CCEnvs.Unity.ECS.Movables
{
    [Serializable]
    public struct MoveDirection : IComponentData
    {
        public float3 Value;
    }
}
