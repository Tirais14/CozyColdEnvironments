using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Diagnostics.CodeAnalysis;

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
            :
            base(target)
        {
            Guard.IsNotNull(target);

            Name = target.Name;
            AssemblyName = target.AssemblyQualifiedName;
        }

        /// <returns><paramref name="target"/></returns>
        public override bool TryRestore(Type? target, [NotNullWhen(true)] out Type? restored)
        {
            if (CanRestore(target))
            {
                restored = null;
                return false;
            }

            restored = Type.GetType($"{Name}, {AssemblyName}");
            return true;
        }

        public override bool CanRestore(Type? target) => true;
    }
}
