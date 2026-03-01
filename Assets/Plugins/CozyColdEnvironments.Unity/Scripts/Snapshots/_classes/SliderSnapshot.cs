using CCEnvs.Attributes.Serialization;
using Newtonsoft.Json;
using System;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public record SliderSnapshot<T> : MonoBehaviourSnapshot<T>
        where T : Slider
    {
        [JsonIgnore]
        [SerializeField]
        protected float? value;

        public float? Value {
            get => value;
            set => this.value = value;
        }

        public SliderSnapshot()
        {
        }

        public SliderSnapshot(T target) : base(target)
        {
        }

        protected SliderSnapshot(MonoBehaviourSnapshot<T> original) : base(original)
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
    [SerializationDescriptor("SliderSnapshot", "ba53286b-2e78-4cce-822b-46fcd2e3bbb2")]
    public record SliderSnapshot : SliderSnapshot<Slider>
    {
        public SliderSnapshot()
        {
        }

        public SliderSnapshot(Slider target) : base(target)
        {
        }

        protected SliderSnapshot(SliderSnapshot<Slider> original) : base(original)
        {
        }

        protected SliderSnapshot(MonoBehaviourSnapshot<Slider> original) : base(original)
        {
        }
    }
}
