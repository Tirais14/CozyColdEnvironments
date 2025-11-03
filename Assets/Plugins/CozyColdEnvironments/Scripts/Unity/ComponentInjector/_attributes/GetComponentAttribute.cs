using CCEnvs.FuncLanguage;
using System;

#nullable enable
namespace CCEnvs.Unity.Injections
{
    public abstract class GetComponentAttribute : Attribute
    {
        public string gameObjectName { get; init; } = null!;
        public bool IsOptional { get; init; }

        public Maybe<string> GameObjectName => gameObjectName;
    }
}
