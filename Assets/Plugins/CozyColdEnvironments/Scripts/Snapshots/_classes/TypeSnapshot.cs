using CCEnvs.Attributes.Serialization;
using System;

#nullable enable
namespace CCEnvs.Snapshots
{
    [Serializable]
    [SerializationDescriptor("TypeSnapshot", "250788e8-a26b-45a3-9abd-f3471a842972")]
    public sealed record TypeSnapshot : Snapshot<Type>
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
        }

        public override bool CanRestore(Type? target) => false;

        protected override Type? CreateValue()
        {
            return Type.GetType($"{Name}, {AssemblyName}", throwOnError: false);
        }

        protected override void OnRestore(ref Type target)
        {
        }

        protected override void OnCapture(Type target)
        {
            base.OnCapture(target);

            Name = target.Name;
            AssemblyName = target.AssemblyQualifiedName;
        }

        protected override void OnReset()
        {
            base.OnReset();

            Name = null;
            AssemblyName = null;
        }
    }
}
