using CCEnvs.Attributes.Serialization;
using CCEnvs.Collections;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using ObservableCollections;
using System;
using System.Collections;
using System.Collections.Generic;

#nullable enable
#pragma warning disable IDE0044
namespace CCEnvs.Saves
{
    [Serializable]
    [SerializationDescriptor("SaveData", "{868DC038-8CB2-4C61-97DE-931D4D21212C}")]
    public sealed class SaveData
        :
        IEnumerable<SaveEntry>,
        IDisposable
    {
        [JsonProperty("saveUnits")]
        private ObservableDictionary<string, SaveEntry> saveEntries = new();

        [JsonProperty("group")]
        public SaveGroup Group { get; init; }

        [JsonIgnore]
        public IReadOnlyObservableDictionary<string, SaveEntry> SaveEntries {
            get => saveEntries;
        }

        public SaveData(SaveGroup group)
        {
            Guard.IsNotNull(group, nameof(group));

            Group = group;
        }

        public bool TryAdd(SaveEntry saveEntry)
        {
            return saveEntries.TryAdd(saveEntry.Key, saveEntry);
        }

        public bool Remove(string key, out SaveEntry saveEntry)
        {
            Guard.IsNotNull(key, nameof(key));

            return saveEntries.Remove(key, out saveEntry);
        }

        public bool Remove(string key)
        {
            Guard.IsNotNull(key, nameof(key));

            return Remove(key);
        }

        public SaveData Clear()
        {
            saveEntries.Clear();

            return this;
        }

        public SaveData AddRange(IEnumerable<SaveEntry> saveEntries)
        {
            CC.Guard.IsNotNull(saveEntries, nameof(saveEntries));

            if (saveEntries.IsEmpty())
                return this;

            foreach (var saveUnit in saveEntries)
                this.saveEntries.TryAdd(saveUnit.Key, saveUnit);

            return this;
        }

        public SaveData Write(
            IEnumerable<SaveEntry> saveEntries,
            WriteSaveDataMode writeSaveDataMode = default
            )
        {
            if (saveEntries.IsNull())
            {
                this.PrintError($"Argument: {nameof(saveEntries)} is null");
                return this;
            }

            switch (writeSaveDataMode)
            {
                case WriteSaveDataMode.Override:
                    Override(saveEntries);
                    break;
                case WriteSaveDataMode.Merge:
                    Merge(saveEntries);
                    break;
                default:
                    throw new InvalidOperationException(writeSaveDataMode.ToString());
            }

            return this;
        }

        public SaveData Merge(IEnumerable<SaveEntry> otherSaveEntries)
        {
            CC.Guard.IsNotNull(otherSaveEntries, nameof(otherSaveEntries));

            if (otherSaveEntries.IsEmpty())
                return this;

            foreach (var saveUnit in otherSaveEntries)
            {
                if (!saveEntries.TryAdd(saveUnit.Key, saveUnit))
                    saveEntries[saveUnit.Key] = saveUnit;
            }

            return this;
        }

        public SaveData Override(IEnumerable<SaveEntry> saveEntries)
        {
            CC.Guard.IsNotNull(saveEntries, nameof(saveEntries));

            this.saveEntries.Clear();

            foreach (var saveUnit in saveEntries)
                this.saveEntries.Add(saveUnit.Key, saveUnit);

            return this;
        }

        public IEnumerator<SaveEntry> GetEnumerator()
        {
            return saveEntries.To<IDictionary<string, SaveEntry>>().Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;
        }
    }
}
