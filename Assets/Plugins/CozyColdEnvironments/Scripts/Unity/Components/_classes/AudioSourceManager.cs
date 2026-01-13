using CCEnvs.Caching;
using CommunityToolkit.Diagnostics;
using Humanizer;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Components
{
    public sealed class AudioSourceManager : CCBehaviourStaticPublic<AudioSourceManager>
    {
        private readonly Cache<string, Item> items = new();
        private readonly AudioSourceRegistryQuery query = new();

        /// <returns>registration which already binded to audio source and regsitry life time</returns>
        public static IDisposable RegisterAudioSource(AudioSourceManagerEntry entry)
        {
            CC.Guard.IsNotNull(entry, nameof(entry));
            Guard.IsNotNull(entry.EntryTag, nameof(entry.EntryTag));

            var item = self.items.GetOrCreate(
                entry.EntryTag,
                factory: static (entry) =>
                {
                    entry.ExpirationTimeRelativeToNow = 10.Minutes();
                    return new Item();
                });

            if (!item.Entries.Add(entry))
                return Disposable.Empty;

            item.ApplySettingsTo(entry);

            return Disposable.Create(entry,
                static (audioSourceEntry) =>
                {
                    UnregisterAudioSource(audioSourceEntry);
                })
                .AddToBehaviour(entry)
                .AddToBehaviour(self);
        }

        public static bool UnregisterAudioSource(AudioSourceManagerEntry entry)
        {
            CC.Guard.IsNotNull(entry, nameof(entry));
            CC.Guard.IsNotNull(entry.EntryTag, nameof(entry.EntryTag));

            if (!self.items.Get(entry.EntryTag).TryGetValue(out var item))
                return false;

            return item.Entries.Remove(entry);
        }

        public static IEnumerable<AudioSourceManagerEntry> GetAudioSourceEntries(
            string? tag,
            bool includeInactive = false)
        {
            IEnumerable<AudioSourceManagerEntry> entries;

            if (tag.IsNullOrWhiteSpace())
                entries = self.items.Values.SelectMany(static item => item.Entries);
            else
            {
                entries = self.items.Get(tag)
                    .Map(static item => (IEnumerable<AudioSourceManagerEntry>)item.Entries)
                    .GetValue(static () => Array.Empty<AudioSourceManagerEntry>());
            }

            if (!includeInactive)
                return entries.Where(static entry => entry.isActiveAndEnabled);

            return entries;
        }

        public static IEnumerable<AudioSource> GetAudioSources(
            string? tag, 
            bool includeInactive = false)
        {
            return GetAudioSourceEntries(tag, includeInactive).Select(static entry => entry.Source);
        }

        public static void SetAudioSoucreVolumeMultiplier(string? tag, float multiplier)
        {
            multiplier = Mathf.Abs(multiplier);
            tag ??= string.Empty;

            var item = self.items.GetOrCreate(
                tag,
                factory: static entry =>
                {
                    entry.ExpirationTimeRelativeToNow = 10.Minutes();
                    return new Item();
                });

            item.VolumeMultiplier = multiplier;

            foreach (var entry in GetAudioSourceEntries(tag, includeInactive: true))
                entry.Source.volume = entry.CapturedState.Volume * multiplier;
        }

        public static float GetAudioSourceVolumeMultiplier(string? tag)
        {
            if (!self.items.Get(tag ?? string.Empty).TryGetValue(out var item))
                return 1f;

            return item.VolumeMultiplier;
        }

        public static AudioSourceRegistryQuery Query(string? tag)
        {
            return self.query.Reset().From(GetAudioSourceEntries(tag));
        }

        public static AudioSourceRegistryQuery Q(string? tag) => Query(tag);

        private sealed class Item
        {
            public HashSet<AudioSourceManagerEntry> Entries { get; } = new();
            public float VolumeMultiplier { get; set; } = 1f;

            public void ApplySettingsTo(AudioSourceManagerEntry entry)
            {
                float volume = entry.CapturedState.Volume * VolumeMultiplier * GetAudioSourceVolumeMultiplier(null);
                entry.Source.volume = volume;
            }
        }
    }
}
