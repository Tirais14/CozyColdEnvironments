using CommunityToolkit.Diagnostics;
using SuperLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Components
{
    public class AudioSourceRegistryQuery
    {
        [Flags]
        public enum Settings : uint
        {
            None,
            IncludeInactive
        }

        private IEnumerable<AudioSourceRegistryEntry>? _rawAudioSourceEntries;

        public IEnumerable<AudioSourceRegistryEntry> AudioSourceEntries { get; set; } = null!;

        public IEnumerable<AudioSourceRegistryEntry> RawAudioSourceEntries {
            get => _rawAudioSourceEntries ?? AudioSourceEntries; 
            private set => _rawAudioSourceEntries = value ?? Array.Empty<AudioSourceRegistryEntry>(); 
        }

        public Settings settings { get; set; }

        public AudioSourceRegistryQuery()
        {
            Reset();
        }

        public AudioSourceRegistryQuery From(IEnumerable<AudioSourceRegistryEntry> entries)
        {
            Guard.IsNotNull(entries, nameof(entries));

            AudioSourceEntries = entries;

            return this;
        }

        public AudioSourceRegistryQuery IncludeInactive(bool state)
        {
            if (state)
                settings |= Settings.IncludeInactive;
            else
                settings &= ~Settings.IncludeInactive;

            return this;
        }

        public void Reset()
        {
            _rawAudioSourceEntries = null;
            AudioSourceEntries = Array.Empty<AudioSourceRegistryEntry>();
        }

        public AudioSourceRegistryQuery VolumeMultiplier(float multiplier)
        {
            multiplier = Mathf.Abs(multiplier);

            RawAudioSourceEntries = AudioSourceEntries.Do(
                aSource =>
                {
                    aSource.Source.volume = Mathf.Clamp01(aSource.Source.volume * multiplier);
                });

            return this;
        }

        public AudioSource[] Materialize()
        {
            if (settings.IsFlagSetted(Settings.IncludeInactive))
            {
                RawAudioSourceEntries = RawAudioSourceEntries.Where(entry =>
                {
                    return entry.Source.isActiveAndEnabled;
                });
            }

            foreach (var entry in RawAudioSourceEntries)
                entry.RestoreAudioSourceState();

            return RawAudioSourceEntries.Select(
                static aSource =>
                {
                    return aSource.Source;
                })
                .ToArray();
        }

        public void MaterializeVoid() => Materialize();
    }
}
