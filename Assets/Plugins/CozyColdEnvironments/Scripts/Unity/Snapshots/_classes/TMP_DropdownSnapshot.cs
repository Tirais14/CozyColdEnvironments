using System;
using System.Diagnostics.CodeAnalysis;
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


        public override bool TryRestore(TMP_Dropdown? target, [NotNullWhen(true)] out TMP_Dropdown? restored)
        {
            if (!base.TryRestore(target, out restored))
                return false;

            target!.value = Value;

            restored = target;
            return true;
        }
    }
}
