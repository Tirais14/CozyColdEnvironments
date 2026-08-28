using System;
using Unity.Entities;

#nullable enable
namespace CCEnvs.UnityX.ECS.Characters
{
    [Serializable]
    public struct CharacterColliderInfo : ISharedComponentData
    {
        public float Height;
        public float Radius;
    }
}
