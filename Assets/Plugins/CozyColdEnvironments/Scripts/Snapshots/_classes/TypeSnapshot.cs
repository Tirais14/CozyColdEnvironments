using CCEnvs.FuncLanguage;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

#nullable enable
namespace CCEnvs.Snapshots
{
    [Serializable]
    public sealed class TypeSnapshot : Snapshot<Type>
    {
        [JsonProperty]
#if UNITY_2017_1_OR_NEWER
        [field: UnityEngine.SerializeField]
#endif
        public string? Name { get; private set; }

        [JsonProperty]
#if UNITY_2017_1_OR_NEWER
        [field: UnityEngine.SerializeField]
#endif
        public string? AssemblyName { get; private set; }

        public override bool IgnoreTarget => true;

        public TypeSnapshot()
        {
        }

        public TypeSnapshot(Type target)
            :
            base(target)
        {
            Guard.IsNotNull(target);

            Name = target.Name;
            AssemblyName = target.AssemblyQualifiedName;
        }

        /// <returns><paramref name="target"/></returns>
        public override Maybe<Type> Restore(Type? target)
        {
            return Type.GetType($"{Name}, {AssemblyName}");
        }
    }
}
