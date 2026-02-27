using System;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public class ToggleSnapshot : SelectableSnapshot<Toggle>
    {
        [SerializeField]
        protected bool isOn;

        public bool IsOn {
            get => isOn;
            set => isOn = value;
        }

        public ToggleSnapshot()
        {
        }

        public ToggleSnapshot(Toggle target) : base(target)
        {
            isOn = target.isOn;
        }

        protected override void OnRestore(ref Toggle target)
        {
            base.OnRestore(ref target);

            target.isOn = isOn;
        }
    }
}
