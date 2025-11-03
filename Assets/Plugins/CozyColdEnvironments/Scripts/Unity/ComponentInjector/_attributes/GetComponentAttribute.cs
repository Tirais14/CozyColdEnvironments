using System;

#nullable enable
namespace CCEnvs.Unity.Injections
{
    public abstract class GetComponentAttribute : Attribute
    {
        public string? Name { get; init; }
        public string? Tag { get; init; }   
        public bool IsOptional { get; init; }
    }
}
