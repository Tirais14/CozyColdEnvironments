using CCEnvs.Attributes.Serialization;
using System;
using TMPro;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public record TMP_DropdownSnapshot<T> : MonoBehaviourSnapshot<T>
        where T : TMP_Dropdown
    {
        [SerializeField]
        protected int? value;

        public int? Value {
            get => value;
            set => this.value = value;
        }

        public TMP_DropdownSnapshot()
        {
        }

        public TMP_DropdownSnapshot(T target) : base(target)
        {
        }

        protected override void OnRestore(ref T target)
        {
            base.OnRestore(ref target);

            if (value.HasValue)
                target.value = value.Value;
        }

        protected override void OnCapture(T target)
        {
            base.OnCapture(target);

            value = target.value;
        }

        protected override void OnReset()
        {
            base.OnReset();

            value = default;
        }
    }

    [Serializable]
    [SerializationDescriptor("TMP_DropdownSnapshot", "ae4298d7-7cf7-4d92-ac28-c005a83e8067")]
    public record TMP_DropdownSnapshot : TMP_DropdownSnapshot<TMP_Dropdown>
    {
        public TMP_DropdownSnapshot()
        {
        }

        public TMP_DropdownSnapshot(TMP_Dropdown target) : base(target)
        {
        }

        protected TMP_DropdownSnapshot(TMP_DropdownSnapshot<TMP_Dropdown> original) : base(original)
        {
        }
    }
}
