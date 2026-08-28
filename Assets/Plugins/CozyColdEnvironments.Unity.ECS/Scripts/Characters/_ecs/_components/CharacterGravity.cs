using System;
using Unity.Entities;

#nullable enable
namespace CCEnvs.UnityX.ECS.Characters
{
    [Serializable]
    public struct CharacterGravity : IComponentData
    {
        public float Value;
    }
}
