using System;
using CommunityToolkit.Diagnostics;

#nullable enable
namespace CCEnvs.Snapshots
{
    [Serializable]
    public sealed class TypeSnapshot : Snapshot<Type>
    {
#if UNITY_2017_1_OR_NEWER
        [field: UnityEngine.Space(8f)]
        [field: UnityEngine.Header(nameof(TypeSnapshot))]

        [field: UnityEngine.SerializeField]
#endif
        public string? Name { get; set; }

#if UNITY_2017_1_OR_NEWER
        [field: UnityEngine.SerializeField]
#endif
        public string? AssemblyName { get; set; }

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

        public override bool CanRestore(Type? target) => false;

        protected override Type? CreateValue()
        {
            return Type.GetType($"{Name}, {AssemblyName}", throwOnError: false);
        }

        protected override void OnRestore(ref Type target)
        {
        }
    }
}
