using System;
using System.Diagnostics.CodeAnalysis;
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

        public override bool TryRestore(
            AudioSource? target, 
            [NotNullWhen(true)] out AudioSource? restored)
        {
            if (!base.TryRestore(target, out restored))
                return false;

            target!.volume = Mathf.Clamp01(Volume);

            restored = target;
            return true;
        }
    }
}
