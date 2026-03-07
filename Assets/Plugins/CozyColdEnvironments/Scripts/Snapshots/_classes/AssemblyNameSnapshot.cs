using System;
using System.Reflection;
using CCEnvs.Attributes.Serialization;

#nullable enable
namespace CCEnvs.Snapshots
{
    [Serializable]
    [SerializationDescriptor("AssemblyNameSnapshot", "a02a1a35-1e09-4e7e-b0bd-45a9df951ab1")]
    public sealed record AssemblyNameSnapshot : Snapshot<AssemblyName>
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
        }

        protected override AssemblyName? CreateValue()
        {
            return new AssemblyName();
        }

        protected override void OnRestore(ref AssemblyName target)
        {
            target.Name = Name;
        }

        protected override void OnCapture(AssemblyName target)
        {
            base.OnCapture(target);

            Name = target.Name;
        }

        protected override void OnReset()
        {
            base.OnReset();

            Name = null;
        }
    }
}
