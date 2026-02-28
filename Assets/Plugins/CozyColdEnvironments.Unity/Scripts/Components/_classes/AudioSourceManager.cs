using CCEnvs.Caching;
using CCEnvs.FuncLanguage;
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
        //private readonly AudioSourceRegistryQuery query = new();

        protected override void Awake()
        {
            base.Awake();
            items.CreateEntry(string.Empty).ExpirationTimeRelativeToNow = null;
        }

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
                .AddDisposableTo(entry)
                .AddDisposableTo(self);
        }

        public static bool UnregisterAudioSource(AudioSourceManagerEntry entry)
        {
            CC.Guard.IsNotNull(entry, nameof(entry));
            CC.Guard.IsNotNull(entry.EntryTag, nameof(entry.EntryTag));

            if (!self.items.Get(entry.EntryTag).Maybe().TryGetValue(out var item))
                return false;

            return item.Entries.Remove(entry);
        }

        /// <returns>LINQ Query</returns>
        public static IEnumerable<AudioSourceManagerEntry> GetAudioSourceEntries(
            string? tag,
            bool includeInactive = false)
        {
            var entries = tag.IsNullOrWhiteSpace() switch
            {
                false => self.items.Get(tag)
                    .Maybe()
                    .Map(static item => (IEnumerable<AudioSourceManagerEntry>)item.Entries)
                    .GetValue(static () => Array.Empty<AudioSourceManagerEntry>()),

                _ => self.items.Values.SelectMany(item => item.Entries)
            };

            if (!includeInactive)
                return entries.Where(static entry => entry.isActiveAndEnabled);

            return entries;
        }

        public static void SetAudioSourceVolumeMultiplier(string? tag, float multiplier)
        {
            NormalizeTag(tag, out tag);

            multiplier = Mathf.Abs(multiplier);

            var item = self.items.GetOrCreate(
                tag,
                factory: static entry =>
                {
                    entry.ExpirationTimeRelativeToNow = 10.Minutes();
                    return new Item();
                });

            item.VolumeMultiplier = multiplier;

            if (IsGeneralTag(tag))
            {
                foreach (var tItem in self.items.Where(item => !IsGeneralTag(item.Key)).Select(item => item.Value))
                    ApplyAudioSourcesVolumeMultiplier(tItem);

                return;
            }

            ApplyAudioSourcesVolumeMultiplier(item);
        }

        public static float GetAudioSourceVolumeMultiplier(string? tag)
        {
            NormalizeTag(tag, out tag);

            if (!self.items.Get(tag).Maybe().TryGetValue(out var item))
                return 1f;

            return item.VolumeMultiplier;
        }

        //public static AudioSourceRegistryQuery Query(string? tag)
        //{
        //    return self.query.Reset().From(GetAudioSourceEntries(tag));
        //}

        //public static AudioSourceRegistryQuery Q(string? tag) => Query(tag);

        private static void ApplyAudioSourcesVolumeMultiplier(
            Item item)
        {
            float gMultiplier = GetAudioSourceVolumeMultiplier(null);

            float multiplier = item.VolumeMultiplier;

            foreach (var entry in item.Entries)
                entry.Source.volume = entry.CapturedState.Volume.GetValueOrDefault() * multiplier * gMultiplier;
        }

        private static void NormalizeTag(string? tag, out string result)
        {
            if (IsGeneralTag(tag))
            {
                result = string.Empty;
                return;
            }

            result = tag;
        }

        private static bool IsGeneralTag(string? tag)
        {
            return tag.IsNullOrWhiteSpace();
        }

        private sealed class Item
        {
            public HashSet<AudioSourceManagerEntry> Entries { get; } = new();
            public float VolumeMultiplier { get; set; } = 1f;

            public void ApplySettingsTo(AudioSourceManagerEntry entry)
            {
                float volume = entry.CapturedState.Volume.GetValueOrDefault() * VolumeMultiplier * GetAudioSourceVolumeMultiplier(null);
                entry.Source.volume = volume;
            }
        }
    }
}
