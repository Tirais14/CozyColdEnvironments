using CCEnvs.Attributes.Serialization;
using CCEnvs.Collections;
using CCEnvs.Diagnostics;
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
    [Serializable, JsonObject]
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
            if (CCDebug.Instance.IsEnabled)
                this.PrintLog($"Save entry added: {saveEntry}");

            var success = saveEntries.TryAdd(saveEntry.Key, saveEntry);

            if (!success && CCDebug.Instance.IsEnabled)
                this.PrintLog($"Save entry not added: {saveEntry}");

            return success;
        }

        public bool Remove(string key, out SaveEntry saveEntry)
        {
            Guard.IsNotNull(key, nameof(key));

            if (CCDebug.Instance.IsEnabled)
                this.PrintLog($"Removing save entry: {key}");

            var success = saveEntries.Remove(key, out saveEntry);

            if (success && CCDebug.Instance.IsEnabled)
                this.PrintLog($"Removed save entry: {saveEntry}");

            return success;
        }

        public bool Remove(string key)
        {
            Guard.IsNotNull(key, nameof(key));

            if (CCDebug.Instance.IsEnabled)
                this.PrintLog($"Removing save entry: {key}");

            var success = saveEntries.Remove(key);

            if (success && CCDebug.Instance.IsEnabled)
                this.PrintLog($"Removed save entry: {key}");

            return success;
        }

        public SaveData Clear()
        {
            if (CCDebug.Instance.IsEnabled)
                this.PrintLog("Clearing...");

            saveEntries.Clear();

            if (CCDebug.Instance.IsEnabled)
                this.PrintLog("Cleared");

            return this;
        }

        public SaveData AddRange(IEnumerable<SaveEntry> saveEntries)
        {
            CC.Guard.IsNotNull(saveEntries, nameof(saveEntries));

            if (CCDebug.Instance.IsEnabled)
                this.PrintLog("Adding...");

            if (saveEntries.IsEmpty())
                return this;

            foreach (var saveUnit in saveEntries)
                this.saveEntries.TryAdd(saveUnit.Key, saveUnit);

            if (CCDebug.Instance.IsEnabled)
                this.PrintLog("Added");

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

            if (CCDebug.Instance.IsEnabled)
                this.PrintLog("Merging...");

            if (otherSaveEntries.IsEmpty())
                return this;

            foreach (var saveEntry in otherSaveEntries)
                saveEntries[saveEntry.Key] = saveEntry;

            if (CCDebug.Instance.IsEnabled)
                this.PrintLog("Merged");

            return this;
        }

        public SaveData Override(IEnumerable<SaveEntry> saveEntries)
        {
            CC.Guard.IsNotNull(saveEntries, nameof(saveEntries));

            if (CCDebug.Instance.IsEnabled)
                this.PrintLog("Overriding...");

            this.saveEntries.Clear();

            foreach (var saveEntry in saveEntries)
                this.saveEntries[saveEntry.Key] = saveEntry;

            if (CCDebug.Instance.IsEnabled)
                this.PrintLog("Overrided");

            return this;
        }

        public string SerializeEntries()
        {
            return JsonConvert.SerializeObject(saveEntries, SaveSystem.SerializerSettings) ?? string.Empty;
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
