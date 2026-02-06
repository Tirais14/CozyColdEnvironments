using CCEnvs.Unity.Injections;
using UnityEngine;
using CCEnvs.Unity.Snapshots;

#nullable enable
namespace CCEnvs.Unity.Components
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class AudioSourceManagerEntry : CCBehaviour
    {
        public const string DEFAULT_TAG = "Undefined";

        [field: SerializeField]
        public string EntryTag { get; private set; } = DEFAULT_TAG;

        [field: GetBySelf]
        public AudioSource Source { get; private set; } = null!;

        public AudioSourceSnapshot CapturedState { get; private set; } = null!;

        protected override void Awake()
        {
            base.Awake();
            CaptureAudioSourceState();
            AudioSourceManager.RegisterAudioSource(this);
        }

        public void CaptureAudioSourceState()
        {
            CapturedState = new AudioSourceSnapshot(Source);
        }

        public void RestoreAudioSourceState()
        {
            CapturedState.TryRestore(Source, out _);
        }
    }
}
