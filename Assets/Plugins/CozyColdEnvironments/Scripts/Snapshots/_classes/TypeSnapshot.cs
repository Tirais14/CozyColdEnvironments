using CCEnvs.FuncLanguage;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using System;

#nullable enable
namespace CCEnvs.Snapshots
{
    [Serializable]
    public struct TypeSnapshot : ISnapshot<Type>
    {
#if UNITY_2017_1_OR_NEWER
        [UnityEngine.SerializeField]
#endif
        [JsonProperty("name")]
        private string name;

#if UNITY_2017_1_OR_NEWER
        [UnityEngine.SerializeField]
#endif
        [JsonProperty("assemblyName")]
        private string assemblyName;

        [JsonIgnore]
        public Maybe<Type> Target { get; private set; }

        public TypeSnapshot(Type target)
        {
            Guard.IsNotNull(target);

            Target = target;
            name = target.Name;
            assemblyName = target.AssemblyQualifiedName;
        }

        public readonly Type Restore() => Restore(null);

        public readonly Type Restore(Type? target)
        {
            return Type.GetType($"{name}, {assemblyName}", throwOnError: true);
        }
    }
}
