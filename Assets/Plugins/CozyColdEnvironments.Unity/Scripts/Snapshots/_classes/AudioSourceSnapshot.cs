using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public class AudioSourceSnapshot : BehaviourSnapshot<AudioSource>
    {
        public float Volume { get; private set; }

        public AudioSourceSnapshot()
        {
        }

        public AudioSourceSnapshot(AudioSource target) : base(target)
        {
            Volume = target.volume;
        }

        protected override void OnRestore(ref AudioSource target)
        {
            base.OnRestore(ref target);

            target!.volume = Mathf.Clamp01(Volume);
        }
    }
}
