using CCEnvs.FuncLanguage;
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

        public override bool IgnoreTarget => false;

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
        public override Maybe<AssemblyName> Restore(AssemblyName? target)
        {
            target ??= new AssemblyName();

            target.Name = Name;

            return target;
        }
    }
}
