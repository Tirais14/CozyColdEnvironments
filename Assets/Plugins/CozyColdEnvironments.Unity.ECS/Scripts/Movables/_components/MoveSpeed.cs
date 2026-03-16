using System;
using Unity.Entities;

#nullable enable
namespace CCEnvs.Unity.ECS.Movables
{
    [Serializable]
    public struct MoveSpeed : IComponentData
    {
        public float Value;
    }
}
