using CCEnvs.Unity.Injections;
using UnityEngine;
using CCEnvs.Unity.Snapshots;

#nullable enable
namespace CCEnvs.Unity.Components
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class AudioSourceRegistryEntry : CCBehaviour
    {
        private AudioSourceSnapshot sourceSnapshot = null!;

        public string EntryTag { get; private set; } = string.Empty;

        [field: GetBySelf]
        public AudioSource Source { get; private set; } = null!;

        protected override void Awake()
        {
            base.Awake();
            AudioSourceRegistry.RegisterAudioSource(this);
        }

        protected override void Start()
        {
            base.Start();
            CaptureAudioSourceState();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            AudioSourceRegistry.UnregisterAudioSource(this);
        }

        public void CaptureAudioSourceState()
        {
            sourceSnapshot = new AudioSourceSnapshot(Source);
        }

        public void RestoreAudioSourceState()
        {
            sourceSnapshot.TryRestore(Source, out _);
        }
    }
}
