using CCEnvs.Caching;
using CCEnvs.Collections;
using CommunityToolkit.Diagnostics;
using Humanizer;
using Microsoft.Extensions.Caching.Memory;
using R3;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Components
{
    public sealed class AudioSourceRegistry : CCBehaviourStaticPublic<AudioSourceRegistry>
    {
        private readonly ReferenceCache<string, HashSet<AudioSourceRegistryEntry>> entryCollections = new()
        {
            ExpirationScanFrequency = 1.Minutes()
        };

        /// <returns>registration which already binded to audio source and regsitry life time</returns>
        public static IDisposable RegisterAudioSource(AudioSourceRegistryEntry audioSourceEntry)
        {
            CC.Guard.IsNotNull(audioSourceEntry, nameof(audioSourceEntry));
            Guard.IsNotNull(audioSourceEntry.EntryTag, nameof(audioSourceEntry.EntryTag));

            var entries = self.entryCollections.GetOrCreate(
                audioSourceEntry.EntryTag,
                factory: static (entry) =>
                {
                    entry.ExpirationTimeRelativeToNow = 10.Minutes();
                    return new HashSet<AudioSourceRegistryEntry>();
                })
                .GetValueUnsafe();

            if (entries.Contains(audioSourceEntry))
                return Disposable.Empty;

            return Disposable.Create(audioSourceEntry,
                static (audioSourceEntry) =>
                {
                    UnregisterAudioSource(audioSourceEntry);
                })
                .AddToBehaviour(self);
        }

        public static bool UnregisterAudioSource(AudioSourceRegistryEntry audioSourceEntry)
        {
            CC.Guard.IsNotNull(audioSourceEntry, nameof(audioSourceEntry));
            CC.Guard.IsNotNull(audioSourceEntry.EntryTag, nameof(audioSourceEntry.EntryTag));

            if (!self.entryCollections.Get(audioSourceEntry.EntryTag).TryGetValue(out var entries))
                return false;

            return entries!.Remove(audioSourceEntry);
        }

        public static IEnumerable<AudioSourceRegistryEntry> GetAudioSourceEntries(string tag)
        {
            Guard.IsNotNull(tag, nameof(tag));

            if (!self.entryCollections.Get(tag).TryGetValue(out var entries))
                return Array.Empty<AudioSourceRegistryEntry>();

            return entries;
        }

        public static IEnumerable<AudioSource> GetAudioSources(
            string tag, 
            bool includeInactive = false)
        {
            CC.Guard.IsNotNull(tag, nameof(tag));

            if (!self.entryCollections.Get(tag).TryGetValue(out var entries))
                return Array.Empty<AudioSource>();

            if (includeInactive)
                return entries.Select(entry => entry.Source);

            return entries.Select(static entry =>
                {
                    return entry.Source;
                })
                .Where(static aSource =>
                {
                    return aSource.enabled;
                });
        }

        public static AudioSourceRegistryQuery Query(string tag)
        {
            return new AudioSourceRegistryQuery().From(GetAudioSourceEntries(tag));
        }

        public static AudioSourceRegistryQuery Q(string tag) => Query(tag);
    }
}
