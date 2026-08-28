#nullable enable
using Unity.Entities;
using Unity.Mathematics;

namespace CCEnvs.UnityX.ECS.Characters
{
    public struct CharacterInputs : IComponentData
    {
        public float2 MoveInput;
    }
}
