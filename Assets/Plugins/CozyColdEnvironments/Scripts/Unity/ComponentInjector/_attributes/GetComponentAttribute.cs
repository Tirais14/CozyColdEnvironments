using System;

#nullable enable
namespace CCEnvs.Unity.Injections
{
    public abstract class GetComponentAttribute : Attribute
    {
        public string? UnityName { get; init; }
        public string? UnityTag { get; init; }   
        public bool IsOptional { get; init; }
    }
}
