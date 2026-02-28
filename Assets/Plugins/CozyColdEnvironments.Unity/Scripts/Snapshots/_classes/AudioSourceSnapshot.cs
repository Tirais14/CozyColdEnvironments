using CCEnvs.Attributes.Serialization;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    [TypeSerializationDescriptor("AudioSourceSnapshot", "378125b2-945a-4b38-8269-e7ad43f8a9f6")]
    public record AudioSourceSnapshot : BehaviourSnapshot<AudioSource>
    {
        public float? Volume { get; set; }

        public AudioSourceSnapshot()
        {
        }

        public AudioSourceSnapshot(AudioSource target) : base(target)
        {
        }

        protected AudioSourceSnapshot(BehaviourSnapshot<AudioSource> original) : base(original)
        {
        }

        protected override void OnRestore(ref AudioSource target)
        {
            base.OnRestore(ref target);

            if (Volume is not null)
                target!.volume = Mathf.Clamp01(Volume.Value);
        }

        protected override void OnCapture(AudioSource target)
        {
            base.OnCapture(target);

            Volume = target.volume;
        }

        protected override void OnReset()
        {
            base.OnReset();

            Volume = default;
        }
    }
}
