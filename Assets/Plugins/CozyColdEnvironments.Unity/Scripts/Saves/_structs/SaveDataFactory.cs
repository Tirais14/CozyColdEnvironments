using CCEnvs.Snapshots;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    public static class SaveDataFactory
    {
        public static SaveData Create(
            IEnumerable<(object obj, string key, Func<object, ISnapshot> converter)> pairs
            )
        {
            CC.Guard.IsNotNull(pairs, nameof(pairs));

            var saveUnits = new Dictionary<string, SaveUnit>();

            SaveUnit saveUnit;

            ISnapshot snapshot;

            foreach (var (obj, key, converter) in pairs)
            {
                snapshot = converter(obj);

                saveUnit = new SaveUnit(snapshot, key);

                saveUnits.Add(saveUnit.Key, saveUnit);
            }

            return new SaveData(saveUnits);
        }
    }
}
