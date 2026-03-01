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
namespace CCEnvs.Unity.Saves
{
    [Serializable]
    [SerializationDescriptor("Saves.SaveData", "{868DC038-8CB2-4C61-97DE-931D4D21212C}")]
    public class SaveData
        :
        IEnumerable<SaveUnit>
    {
        [JsonProperty("saveUnits")]
        private ObservableDictionary<string, SaveUnit> saveUnits = new();

        [JsonProperty("group")]
        public SaveGroup Group { get; init; }

        [JsonIgnore]
        public IReadOnlyObservableDictionary<string, SaveUnit> SaveUnits {
            get => saveUnits;
        }

        public SaveData(SaveGroup group)
        {
            Guard.IsNotNull(group, nameof(group));

            Group = group;
        }

        public bool TryAdd(SaveUnit saveUnit)
        {
            return saveUnits.TryAdd(saveUnit.Key, saveUnit);
        }

        public bool Remove(string key, out SaveUnit saveUnit)
        {
            Guard.IsNotNull(key, nameof(key));

            return saveUnits.Remove(key, out saveUnit);
        }

        public bool Remove(string key)
        {
            Guard.IsNotNull(key, nameof(key));

            return Remove(key);
        }

        public SaveData Clear()
        {
            saveUnits.Clear();

            return this;
        }

        public SaveData AddRange(IEnumerable<SaveUnit> saveUnits)
        {
            CC.Guard.IsNotNull(saveUnits, nameof(saveUnits));

            if (saveUnits.IsEmpty())
                return this;

            foreach (var saveUnit in saveUnits)
                this.saveUnits.TryAdd(saveUnit.Key, saveUnit);

            return this;
        }

        public SaveData Write(
            IEnumerable<SaveUnit> saveUnits,
            WriteSaveDataMode writeSaveDataMode = default
            )
        {
            if (saveUnits.IsNull())
            {
                this.PrintError($"Argument: {nameof(saveUnits)} is null");
                return this;
            }

            switch (writeSaveDataMode)
            {
                case WriteSaveDataMode.Override:
                    Override(saveUnits);
                    break;
                case WriteSaveDataMode.Merge:
                    Merge(saveUnits);
                    break;
                default:
                    throw new InvalidOperationException(writeSaveDataMode.ToString());
            }

            return this;
        }

        public SaveData Merge(IEnumerable<SaveUnit> otherSaveUnits)
        {
            CC.Guard.IsNotNull(otherSaveUnits, nameof(otherSaveUnits));

            if (otherSaveUnits.IsEmpty())
                return this;

            foreach (var saveUnit in otherSaveUnits)
            {
                if (!saveUnits.TryAdd(saveUnit.Key, saveUnit))
                    saveUnits[saveUnit.Key] = saveUnit;
            }

            return this;
        }

        public SaveData Override(IEnumerable<SaveUnit> saveUnits)
        {
            CC.Guard.IsNotNull(saveUnits, nameof(saveUnits));

            this.saveUnits.Clear();

            foreach (var saveUnit in saveUnits)
                this.saveUnits.Add(saveUnit.Key, saveUnit);

            return this;
        }

        public IEnumerator<SaveUnit> GetEnumerator()
        {
            return saveUnits.To<IDictionary<string, SaveUnit>>().Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
