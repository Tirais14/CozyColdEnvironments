using CCEnvs.Collections;
using CommunityToolkit.Diagnostics;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Components
{
    public sealed class AudioSourceRegistry : CCBehaviourStaticPublic<AudioSourceRegistry>
    {
        private readonly Dictionary<string, HashSet<AudioSourceRegistryEntry>> entryCollections = new();

        /// <returns>registration which already binded to audio source and regsitry life time</returns>
        public static IDisposable RegisterAudioSource(AudioSourceRegistryEntry audioSourceEntry)
        {
            CC.Guard.IsNotNull(audioSourceEntry, nameof(audioSourceEntry));
            Guard.IsNotNull(audioSourceEntry.EntryTag, nameof(audioSourceEntry.EntryTag));

            var entries = self.entryCollections.GetOrCreateNew(audioSourceEntry.EntryTag);

            if (!entries.Add(audioSourceEntry))
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

            if (!self.entryCollections.TryGetValue(audioSourceEntry.EntryTag, out var entries))
                return false;

            return entries.Remove(audioSourceEntry);
        }

        public static IEnumerable<AudioSource> GetAudioSources(
            string tag, 
            bool includeInactive = false)
        {
            CC.Guard.IsNotNull(tag, nameof(tag));

            if (self.entryCollections.TryGetValue(tag, out var entries))
                return Array.Empty<AudioSource>();

            if (includeInactive)
                return entries.Select(entry => entry.Source);

            return from entry in entries
                   select entry.Source into aSource
                   where aSource.enabled
                   select aSource;
        }

        public static AudioSourceRegistryQuery Query(string tag)
        {
            return new AudioSourceRegistryQuery()
            {
                AudioSourceEntries = GetAudioSources(tag),
            };
        }

        public static AudioSourceRegistryQuery Q(string tag) => Query(tag);
    }
}
