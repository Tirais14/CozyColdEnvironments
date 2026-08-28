using System;
using Unity.Entities;

#nullable enable
namespace CCEnvs.UnityX.ECS.Characters
{
    [Serializable]
    public struct CharacterMoveSpeed : IComponentData
    {
        public float Value;
    }
}
