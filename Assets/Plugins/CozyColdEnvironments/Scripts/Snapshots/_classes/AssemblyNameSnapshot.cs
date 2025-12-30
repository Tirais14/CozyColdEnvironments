using CCEnvs.FuncLanguage;
using CommunityToolkit.Diagnostics;
using System;
using System.Diagnostics.CodeAnalysis;
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

        /// <returns><paramref name="target"/></returns>
        public override bool Restore(
            AssemblyName? target,
            [NotNullWhen(true)] out AssemblyName? restored)
        {
            if (!CanRestore(target))
            {
                restored = null;
                return false;
            }

            target ??= new AssemblyName();
            target.Name = Name;

            restored = target;
            return true;
        }
    }
}
