using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Components
{
    public sealed class AuidioSourceManagerProxi : CCBehaviour
    {
        [SerializeField]
        private string audioSourceTag = AudioSourceManagerEntry.DEFAULT_TAG;

        public void SetAudioVolume(float volumeMupltiplier)
        {
            AudioSourceManager.SetAudioSourceVolumeMultiplier(audioSourceTag, volumeMupltiplier);
        }

        public void SetAudioVolume(string rawVolumeMultiplier)
        {
            SetAudioVolume(float.Parse(rawVolumeMultiplier));
        }
    }
}
