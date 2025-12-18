using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
        [JsonProperty]
        private string name;

#if UNITY_2017_1_OR_NEWER
        [UnityEngine.SerializeField]
#endif
        [JsonProperty]
        private string assemblyName;

        [JsonIgnore]
        public Maybe<Type> Target { get; private set; }

        [JsonProperty]
        public string SelfTypeReference { get; private set; }

        public TypeSnapshot(Type target)
            :
            this()
        {
            Guard.IsNotNull(target);

            Target = target;
            SelfTypeReference = GetType().GetTypeReference();
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
