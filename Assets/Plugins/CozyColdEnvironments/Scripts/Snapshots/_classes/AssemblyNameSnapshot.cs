using CCEnvs.FuncLanguage;
using CCEnvs.Reflection;
using CommunityToolkit.Diagnostics;
using System;
using System.Reflection;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

#nullable enable
namespace CCEnvs.Snapshots
{
    [Serializable]
    public struct AssemblyNameSnapshot : ISnapshot<AssemblyName>
    {
#if UNITY_2017_1_OR_NEWER
        [UnityEngine.SerializeField]
#endif
        [JsonProperty]
        private string name;

        [JsonIgnore]
        public Maybe<AssemblyName> Target { get; private set; }

        [JsonProperty]
        public string SelfTypeReference { get; private set; }

        public AssemblyNameSnapshot(AssemblyName target)
            :
            this()
        {
            Guard.IsNotNull(target);

            Target = target;
            SelfTypeReference = GetType().GetTypeReference();
            name = target.Name;
        }

        public readonly AssemblyName Restore() => Restore(Target.Raw);

        public readonly AssemblyName Restore(AssemblyName? target)
        {
            target ??= new AssemblyName();
            target.Name = name;

            return target;
        }
    }
}
