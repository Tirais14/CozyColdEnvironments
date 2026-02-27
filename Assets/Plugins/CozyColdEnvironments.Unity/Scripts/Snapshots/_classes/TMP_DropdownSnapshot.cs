using System;
using TMPro;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public class TMP_DropdownSnapshot : MonoBehaviourSnapshot<TMP_Dropdown>
    {
        [SerializeField]
        protected int value;

        public int Value {
            get => value;
            set => this.value = value;
        }

        public TMP_DropdownSnapshot()
        {
        }

        public TMP_DropdownSnapshot(TMP_Dropdown target) : base(target)
        {
            value = target.value;
        }

        protected override void OnRestore(ref TMP_Dropdown target)
        {
            base.OnRestore(ref target);

            target.value = value;
        }
    }
}
