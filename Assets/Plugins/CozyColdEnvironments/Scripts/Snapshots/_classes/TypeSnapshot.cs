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

        public TypeSnapshot()
        {
        }

        public TypeSnapshot(Type target)
        {
            Guard.IsNotNull(target);

            Target = target;
            Name = target.Name;
            AssemblyName = target.AssemblyQualifiedName;
        }

        public override Type Restore()
        {
            return Type.GetType($"{Name}, {AssemblyName}");
        }

        /// <returns>input type</returns>
        public override Type Restore(Type target) => target;
    }
}
