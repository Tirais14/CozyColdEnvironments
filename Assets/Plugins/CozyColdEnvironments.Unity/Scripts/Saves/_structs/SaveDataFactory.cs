using CCEnvs.Snapshots;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    public static class SaveDataFactory
    {
        public static SaveData Create(
            IEnumerable<(KeyValuePair<string, object> obj, Func<object, ISnapshot> converter)> pairs)
        {
            CC.Guard.IsNotNull(pairs, nameof(pairs));

            var saveUnits = new List<SaveUnit>();

            SaveUnit saveUnit;

            ISnapshot snapshot;

            foreach (var (obj, converter) in pairs)
            {
                snapshot = converter(obj.Value);

                saveUnit = new SaveUnit(snapshot, obj.Key);

                saveUnits.Add(saveUnit);
            }

            return new SaveData(saveUnits);
        }
    }
}
