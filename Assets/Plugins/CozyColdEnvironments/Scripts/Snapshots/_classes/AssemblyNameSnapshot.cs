using CommunityToolkit.Diagnostics;
using System;
using System.Reflection;

#nullable enable
namespace CCEnvs.Snapshots
{
    [Serializable]
    public sealed class AssemblyNameSnapshot : Snapshot<AssemblyName>
    {
#if UNITY_2017_1_OR_NEWER
        [field: UnityEngine.SerializeField]
#endif
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

        protected override AssemblyName? CreateValue()
        {
            return new AssemblyName();
        }

        protected override void OnRestore(ref AssemblyName target)
        {
            target.Name = Name;
        }
    }
}
