using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public class SliderSnapshot : MonoBehaviourSnapshot<Slider>
    {
        [SerializeField]
        protected float value;

        public float Value {
            get => value;
            set => this.value = value;
        }

        public SliderSnapshot()
        {
        }

        public SliderSnapshot(Slider target) : base(target)
        {
            value = target.value;
        }

        public override bool TryRestore(Slider? target, [NotNullWhen(true)] out Slider? restored)
        {
            if (!base.TryRestore(target, out restored))
                return false;

            target!.value = Value;

            restored = target;
            return true;
        }
    }
}
