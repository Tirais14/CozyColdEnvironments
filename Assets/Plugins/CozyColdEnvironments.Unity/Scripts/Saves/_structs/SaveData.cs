using CCEnvs.Attributes.Serialization;
using CCEnvs.Collections;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    [Serializable]
    [TypeSerializationDescriptor("Saves.SaveData", "{868DC038-8CB2-4C61-97DE-931D4D21212C}")]
    public class SaveData
    {
        [JsonProperty("group")]
        public SaveGroup Group { get; }

        [JsonProperty("saveUnits")]
        public Dictionary<string, SaveUnit> SaveUnits { get; }

        public SaveData(SaveGroup group)
        {
            Guard.IsNotNull(group, nameof(group));

            SaveUnits = new Dictionary<string, SaveUnit>();

            Group = group;
        }

        [JsonConstructor]
        public SaveData(
            SaveGroup group,
            IEnumerable<KeyValuePair<string, SaveUnit>>? saveUnits
            )
            :
            this(group)
        {
            if (saveUnits.IsNotNullOrEmpty())
                SaveUnits.AddRange(saveUnits);
        }

        public SaveData Clear()
        {
            SaveUnits.Clear();

            return this;
        }

        public SaveData Fill(IEnumerable<(string Key, SaveUnit Value)> saveUnits)
        {
            CC.Guard.IsNotNull(saveUnits, nameof(saveUnits));

            if (saveUnits.IsEmpty())
                return this;

            foreach (var (key, value) in saveUnits)
                SaveUnits.TryAdd(key, value);

            return this;
        }

        public SaveData Merge(IEnumerable<(string Key, SaveUnit Value)> otherSaveUnits)
        {
            CC.Guard.IsNotNull(otherSaveUnits, nameof(otherSaveUnits));

            if (otherSaveUnits.IsEmpty())
                return this;

            foreach (var (key, value) in otherSaveUnits)
            {
                if (SaveUnits.ContainsKey(key))
                {
                    SaveUnits[key] = value;
                    continue;
                }

                SaveUnits.Add(key, value);
            }

            return this;
        }

        public SaveData Override(IEnumerable<(string Key, SaveUnit Value)> saveUnits)
        {
            CC.Guard.IsNotNull(saveUnits, nameof(saveUnits));

            if (saveUnits.IsEmpty())
                return this;

            foreach (var (key, value) in saveUnits)
            {
                if (!SaveUnits.TryAdd(key, value))
                    SaveUnits[key] = value;
            }

            return this;
        }
    }
}
