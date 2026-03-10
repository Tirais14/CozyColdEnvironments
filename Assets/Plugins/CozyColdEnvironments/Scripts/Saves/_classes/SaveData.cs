using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CCEnvs.Attributes.Serialization;
using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using ObservableCollections;
using R3;

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
        [JsonProperty("saveUnits", Order = 10)]
        private ObservableDictionary<string, SaveEntry> saveEntries = new();

        [JsonIgnore]
        private ReactiveCommand<WriteSaveDataMode> writeEvent = new();

        [JsonIgnore]
        public IReadOnlyObservableDictionary<string, SaveEntry> SaveEntries {
            get => saveEntries;
        }

        [JsonProperty("version", Order = -2)]
        public long Version { get; init; }

        [JsonProperty("groupName", Order = -1)]
        public string GroupName { get; init; }

        public SaveData(string groupName, long version = 0L)
        {
            Guard.IsNotNull(groupName, nameof(groupName));

            GroupName = groupName;
            Version = version;
        }

        public SaveData(SaveGroup group, long version = 0L)
            :
            this(group?.Name!, version)
        {
        }

        public bool Remove(string key, out SaveEntry saveEntry)
        {
            if (key.IsNull())
            {
                this.PrintError($"Argument: {nameof(key)} is null");
                saveEntry = default;
                return false;
            }

            if (CCDebug.Instance.IsEnabled)
                this.PrintLog($"Removing save entry: {key}");

            var success = saveEntries.Remove(key, out saveEntry);

            if (success && CCDebug.Instance.IsEnabled)
                this.PrintLog($"Removed save entry: {saveEntry}");

            return success;
        }

        public bool Remove(string key)
        {
            if (key.IsNull())
            {
                this.PrintError($"Argument: {nameof(key)} is null");
                return false;
            }

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

        public bool Append(SaveEntry saveEntry)
        {
            if (CCDebug.Instance.IsEnabled)
                this.PrintLog($"Save entry not added: {saveEntry}");

            var success = saveEntries.TryAdd(saveEntry.Key, saveEntry);

            if (!success && CCDebug.Instance.IsEnabled)
                this.PrintLog($"Save entry added: {saveEntry}");

            return success;
        }

        public SaveData Append(IEnumerable<SaveEntry> otherSaveEntries)
        {
            if (otherSaveEntries.IsNull())
            {
                this.PrintError($"Argument: {nameof(otherSaveEntries)} is null");
                return this;
            }

            if (CCDebug.Instance.IsEnabled)
                this.PrintLog("Appending...");

            if (otherSaveEntries.IsEmpty())
                return this;

            using var otherSaveEntriesCopy = otherSaveEntries.EnumerableToArrayPooled();

            foreach (var saveUnit in otherSaveEntriesCopy)
                this.saveEntries.TryAdd(saveUnit.Key, saveUnit);

            if (CCDebug.Instance.IsEnabled)
                this.PrintLog("Appended");

            return this;
        }

        public SaveData Write(
            IEnumerable<SaveEntry> otherSaveEntries,
            WriteSaveDataMode writeSaveDataMode = default
            )
        {
            if (otherSaveEntries.IsNull())
            {
                this.PrintError($"Argument: {nameof(otherSaveEntries)} is null");
                return this;
            }

            switch (writeSaveDataMode)
            {
                case WriteSaveDataMode.Override:
                    Override(otherSaveEntries);
                    break;
                case WriteSaveDataMode.Merge:
                    Merge(otherSaveEntries);
                    break;
                case WriteSaveDataMode.Append:
                    Append(otherSaveEntries);
                    break;
                default:
                    throw new InvalidOperationException(writeSaveDataMode.ToString());
            }

            return this;
        }

        public SaveData Merge(IEnumerable<SaveEntry> otherSaveEntries)
        {
            if (otherSaveEntries.IsNull())
            {
                this.PrintError($"Argument: {nameof(otherSaveEntries)} is null");
                return this;
            }

            if (CCDebug.Instance.IsEnabled)
                this.PrintLog("Merging...");

            if (otherSaveEntries.IsEmpty())
                return this;

            using var otherSaveEntriesCopy = otherSaveEntries.EnumerableToArrayPooled();

            foreach (var saveEntry in otherSaveEntriesCopy)
                saveEntries[saveEntry.Key] = saveEntry;

            if (CCDebug.Instance.IsEnabled)
                this.PrintLog("Merged");

            writeEvent?.Execute(WriteSaveDataMode.Merge);

            return this;
        }

        public SaveData Override(IEnumerable<SaveEntry> otherSaveEntries)
        {
            if (otherSaveEntries.IsNull())
            {
                this.PrintError($"Argument: {nameof(otherSaveEntries)} is null");
                return this;
            }

            if (CCDebug.Instance.IsEnabled)
                this.PrintLog("Overriding...");

            this.saveEntries.Clear();

            using var otherSaveEntriesCopy = otherSaveEntries.EnumerableToArrayPooled();

            foreach (var saveEntry in otherSaveEntriesCopy)
                this.saveEntries[saveEntry.Key] = saveEntry;

            if (CCDebug.Instance.IsEnabled)
                this.PrintLog("Overrided");

            writeEvent?.Execute(WriteSaveDataMode.Override);

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

        public void Dispose()
        {
            writeEvent?.Dispose();
        }

        public Observable<WriteSaveDataMode> ObserveWrite()
        {
            writeEvent ??= new ReactiveCommand<WriteSaveDataMode>();

            return writeEvent;
        }
    }
}
