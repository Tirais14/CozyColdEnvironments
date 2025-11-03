using System;

#nullable enable
namespace CCEnvs.Unity.Injections
{
    public abstract class GetComponentAttribute : Attribute
    {
        public string GameObjectName { get; init; } = null!;
        public bool IsOptional { get; init; }
    }
}
