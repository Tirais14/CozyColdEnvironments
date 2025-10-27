using CCEnvs.FuncLanguage;
using System;

#nullable enable
namespace CCEnvs.Unity.Injections
{
    public abstract class GetComponentAttribute : Attribute
    {
        public Maybe<string> GameObjectName { get; init; }
        public bool IsOptional { get; init; }
    }
}
