using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Diagnostics;
using SuperLinq;
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

        private IEnumerable<AudioSourceManagerEntry>? _rawAudioSourceEntries;

        public IEnumerable<AudioSourceManagerEntry> AudioSourceEntries { get; set; } = null!;

        public IEnumerable<AudioSourceManagerEntry> RawAudioSourceEntries {
            get => _rawAudioSourceEntries ?? AudioSourceEntries;
            private set => _rawAudioSourceEntries = value ?? Array.Empty<AudioSourceManagerEntry>();
        }

        public Settings settings { get; set; }

        public AudioSourceRegistryQuery()
        {
            Reset();
        }

        public AudioSourceRegistryQuery From(IEnumerable<AudioSourceManagerEntry> entries)
        {
            Guard.IsNotNull(entries, nameof(entries));

            AudioSourceEntries = entries;

            return this;
        }

        public AudioSourceRegistryQuery IncludeInactive(bool state = true)
        {
            if (state)
                settings |= Settings.IncludeInactive;
            else
                settings &= ~Settings.IncludeInactive;

            return this;
        }

        public AudioSourceRegistryQuery Reset()
        {
            _rawAudioSourceEntries = null;
            AudioSourceEntries = Array.Empty<AudioSourceManagerEntry>();

            return this;
        }

        public AudioSourceRegistryQuery VolumeMultiplier(float multiplier)
        {
            multiplier = Mathf.Abs(multiplier);

            RawAudioSourceEntries = AudioSourceEntries.Do(
                entry =>
                {
                    entry.Source.volume = Mathf.Clamp01(entry.CapturedState.Volume.GetValueOrDefault() * multiplier);
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
