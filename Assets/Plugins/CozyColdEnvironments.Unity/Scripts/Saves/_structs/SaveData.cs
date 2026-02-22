using CCEnvs.Attributes.Serialization;
using CCEnvs.Collections;
using CCEnvs.Pools;
using CCEnvs.Snapshots;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using SuperLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using ZLinq;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    [Serializable]
    [TypeSerializationDescriptor("Saves.SaveData", "{868DC038-8CB2-4C61-97DE-931D4D21212C}")]
    public class SaveData
    {
        [JsonProperty]
        public Dictionary<string, SaveUnit> SaveUnits { get; }

        public SaveData()
        {
            SaveUnits = new Dictionary<string, SaveUnit>();
        }

        [JsonConstructor]
        public SaveData(IEnumerable<KeyValuePair<string, SaveUnit>> saveUnits)
        {
            CC.Guard.IsNotNull(saveUnits, nameof(saveUnits));

            SaveUnits = saveUnits.ToDictionary();
        }

        public SaveData(IEnumerable<SaveUnit> saveUnits)
        {
            CC.Guard.IsNotNull(saveUnits, nameof(saveUnits));

            SaveUnits = saveUnits.ToDictionary(unit => unit.Key);
        }

        public SaveData Clear()
        {
            SaveUnits.Clear();

            return this;
        }

        public SaveData Fill(SaveGroup saveGroup)
        {
            Guard.IsNotNull(saveGroup, nameof(saveGroup));

            using var saveUnits = ListPool<SaveUnit>.Shared.Get();

            Type objType;

            Func<object, ISnapshot>? converter;

            SaveUnit saveUnit;

            ISnapshot snapshot;

            foreach (var item in saveGroup.ObservableObjects)
            {
                objType = item.Value.GetType();

                converter = getConverter(objType);

                if (converter is null)
                    continue;

                snapshot = converter(item.Value);

                saveUnit = new SaveUnit(snapshot, item.Key);

                saveUnits.Value.Add(saveUnit);
            }

            return Merge(saveUnits.Value.Select(saveUnit => (saveUnit.Key, saveUnit)));

            static Func<object, ISnapshot>? getConverter(Type objType)
            {
                if (!SaveSystem.Converters.TryGetValue(objType, out var converter))
                {
                    typeof(SaveDataFactory).PrintLog($"Selected default snapshot converter by: {objType}");

                    return null;
                }

                return converter;
            }
        }

        public SaveData Merge(IEnumerable<(string Key, SaveUnit Value)> otherSaveUnits)
        {
            CC.Guard.IsNotNull(otherSaveUnits, nameof(otherSaveUnits));

            if (otherSaveUnits.IsEmpty())
                return this;

            foreach (var saveUnit in otherSaveUnits)
            {
                if (SaveUnits.ContainsKey(saveUnit.Key))
                {
                    SaveUnits[saveUnit.Key] = saveUnit.Value;
                    continue;
                }

                SaveUnits.Add(saveUnit.Key, saveUnit.Value);
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
