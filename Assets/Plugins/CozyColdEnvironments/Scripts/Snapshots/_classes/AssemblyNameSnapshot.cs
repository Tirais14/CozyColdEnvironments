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
    public sealed class AssemblyNameSnapshot : Snapshot<AssemblyName>
    {
#if UNITY_2017_1_OR_NEWER
        [UnityEngine.SerializeField]
#endif
        [JsonProperty]
        public string? Name { get; private set; }

        public AssemblyNameSnapshot()
        {
        }

        public AssemblyNameSnapshot(AssemblyName target)
            :
            base(target)
        {
            Guard.IsNotNull(target);

            Name = target.Name;
        }

        /// <returns><paramref name="target"/></returns>
        public override AssemblyName Restore(AssemblyName target)
        {
            target ??= new AssemblyName();

            target.Name = Name;

            return target;
        }
    }
}
