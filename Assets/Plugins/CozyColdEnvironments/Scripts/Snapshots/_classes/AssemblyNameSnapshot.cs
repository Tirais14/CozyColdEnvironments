using CCEnvs.FuncLanguage;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using System;
using System.Reflection;

#nullable enable
namespace CCEnvs.Snapshots
{
    [Serializable]
    public struct AssemblyNameSnapshot : ISnapshot<AssemblyName>
    {
#if UNITY_2017_1_OR_NEWER
        [UnityEngine.SerializeField]
#endif
        [JsonProperty("name")]
        private string name;

        public Maybe<AssemblyName> Target { get; private set; }

        public AssemblyNameSnapshot(AssemblyName target)
        {
            Guard.IsNotNull(target);

            Target = target;
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
