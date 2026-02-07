using Newtonsoft.Json;
using System;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public class SliderSnapshot : MonoBehaviourSnapshot<Slider>
    {
        [JsonIgnore]
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

        protected override void OnRestore(ref Slider target)
        {
            base.OnRestore(ref target);

            target.value = value;
        }
    }
}
